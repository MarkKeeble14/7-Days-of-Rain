using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public struct DayData
{
    public WeatherType WeatherType;
    public Dialogue[] ForcedAsleepDialogue;
    public ToDoListItemData[] ToDo;
}

public enum PlayerLocationState
{
    SPAWN,
    EXTERIOR,
    INTERIOR,
    BEDSIDE
}

[System.Serializable]
public struct PlayerLocationStateEntryData
{
    public SerializableDictionary<GameObjectState, GameObject[]> ObjectStateChanges;
    public Vector3 PlayerPosition;
    public Vector3 PlayerEulerAngles;
    public CameraFollowSubject Camera;
}

public class GameManager : MonoBehaviour
{
    public PlayerLocationState CurrentLocationState { get; private set; }
    [SerializeField] private SerializableDictionary<PlayerLocationState, PlayerLocationStateEntryData> playerLocationStateData;
    [SerializeField] private SerializableDictionary<PlayerLocationState, bool> isPlayerLocationStateInsideMap = new SerializableDictionary<PlayerLocationState, bool>();
    [SerializeField] private DayData[] dayDataMap = new DayData[7];
    public int CurrentDay { get; set; }

    public bool PlayerIsInside => isPlayerLocationStateInsideMap[CurrentLocationState];

    public static GameManager _Instance { get; private set; }
    public CameraFollowSubject ActiveCamera => playerLocationStateData[CurrentLocationState].Camera;
    public bool LockMovement { get; set; }
    public bool GameStarted { get; private set; }

    [Header("Sleep")]
    [SerializeField] private float delayAfterForceSleepDialogue = 2;
    public bool BeingForcedToSleep { get; private set; }
    public Action OnEndOfDay { get; set; }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerInput p_Input;
    [SerializeField] private GameEventTrigger bedTrigger;
    [SerializeField] private Light[] interiorLights;
    [SerializeField] private PlayAudioGameEvent doorCloseSoundGameEvent;

    [Header("Cold")]
    [SerializeField] private Image coldBar;
    [SerializeField] private float accumulateColdRate;
    [SerializeField] private float warmUpRate;
    [SerializeField] private float maxColdValue;
    [SerializeField] private Dialogue[] tooColdDialogue;
    [SerializeField] private float delayAfterTooColdDialogue = 2;
    private float currentPlayerCold;
    public float PercentColdValue => currentPlayerCold / maxColdValue;
    public bool BeingForcedInsideForBeingTooCold { get; private set; }

    [Header("Start")]
    [SerializeField] private GameEventTriggerOnKeyPress GetUpFromStartSeatTrigger;
    [SerializeField] private GameEventTriggerOnKeyPress SitDownInStartSeatTrigger;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private CanvasGroup mainMenuCV;
    [SerializeField] private float fadeUIRate;
    [SerializeField] private CanvasGroup inGameUICV;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float exteriorAmbienceVol;
    [SerializeField] private float interiorAmbienceVol;
    [SerializeField] private float exteriorLowpassFreq;
    [SerializeField] private float interiorLowpassFreq;

    [Header("Day Report")]
    [SerializeField] private TextMeshProUGUI dayReportText;
    [SerializeField] private CanvasGroup dayReportCV;
    [SerializeField] private KeyCode closeDayResultScreenKey = KeyCode.Return;

    [Header("To Do")]
    [SerializeField] private ToDoList toDoList;
    [SerializeField] ToDoListItemData[] repeatedTasks;

    // Tracking Crops
    public int CropCount { get; set; }
    private int totalCrops;

    [SerializeField] private Journal journal;

    public IEnumerator SetStateOfInteriorLights(GameObjectState state)
    {
        foreach (Light light in interiorLights)
        {
            light.enabled = state == GameObjectState.ACTIVE ? true : false;
            yield return null;
        }
    }

    private void Awake()
    {
        _Instance = this;

        LoadState(CurrentLocationState);
    }

    private void Start()
    {
        SetDayData();

        SitDownInStartSeatTrigger.Trigger();
        ShowGameEventTriggerOpporotunity._Instance.TryRemoveTrigger(SitDownInStartSeatTrigger);

        ShowGameEventTriggerOpporotunity._Instance.Enable = false;
        p_Input.LockInput = true;

    }

    private void Update()
    {
        if (!GameStarted) return;
        if (BeingForcedInsideForBeingTooCold || BeingForcedToSleep) return;

        if (!PlayerIsInside && currentPlayerCold < maxColdValue)
        {
            currentPlayerCold += Time.deltaTime * accumulateColdRate;
            if (currentPlayerCold > maxColdValue)
            {
                StartCoroutine(TooColdSequence());
                currentPlayerCold = maxColdValue;
            }
        }
        else if (PlayerIsInside && currentPlayerCold > 0)
        {
            currentPlayerCold -= Time.deltaTime * warmUpRate;
            if (currentPlayerCold < 0) currentPlayerCold = 0;
        }
        coldBar.fillAmount = PercentColdValue;
    }

    public void LoadLocationState(PlayerLocationState state, bool playAudio)
    {
        if (playAudio &&
            ((isPlayerLocationStateInsideMap[CurrentLocationState] && !isPlayerLocationStateInsideMap[state])
            || (!isPlayerLocationStateInsideMap[CurrentLocationState] && isPlayerLocationStateInsideMap[state])))
        {
            doorCloseSoundGameEvent.CallActivate();
        }
        CurrentLocationState = state;
        LoadState(CurrentLocationState);


        // Reset the Weather as there are some audio sources that will only play while inside
        WeatherManager._Instance.SetWeather(dayDataMap[CurrentDay].WeatherType);
    }

    private void LoadState(PlayerLocationState state)
    {
        PlayerLocationStateEntryData data = playerLocationStateData[state];

        // Position
        playerTransform.localEulerAngles = data.PlayerEulerAngles;
        playerTransform.position = data.PlayerPosition;

        data.Camera.SetToTargetPos();
        data.Camera.transform.localEulerAngles = playerTransform.localEulerAngles;

        // Enable/Disable Objects
        EnableForGivenState(data);

        if (PlayerIsInside)
        {
            audioMixer.SetFloat("AmbienceVol", Utils.LinearToDecibel(interiorAmbienceVol));
            audioMixer.SetFloat("AmbienceLowPassFreq", interiorLowpassFreq);
            audioMixer.SetFloat("ExternalSFXLowPassFreq", interiorLowpassFreq);
        }
        else
        {
            audioMixer.SetFloat("AmbienceVol", Utils.LinearToDecibel(exteriorAmbienceVol));
            audioMixer.SetFloat("AmbienceLowPassFreq", exteriorLowpassFreq);
            audioMixer.SetFloat("ExternalSFXLowPassFreq", exteriorLowpassFreq);
        }
    }

    private void EnableForGivenState(PlayerLocationStateEntryData data)
    {
        if (data.ObjectStateChanges.ContainsKey(GameObjectState.ACTIVE))
        {
            foreach (GameObject obj in data.ObjectStateChanges[GameObjectState.ACTIVE])
            {
                obj.SetActive(true);
            }
        }
        if (data.ObjectStateChanges.ContainsKey(GameObjectState.INACTIVE))
        {
            foreach (GameObject obj in data.ObjectStateChanges[GameObjectState.INACTIVE])
            {
                obj.SetActive(false);
            }
        }
    }

    public void Slept()
    {
        CurrentDay++;
        SetDayData();

        DayNightManager._Instance.ResetCanForceSleep();
        DayNightManager._Instance.EnableTimeFlow = true;
        DayNightManager._Instance.ResetHasCrossedOutToday();
        BeingForcedToSleep = false;

        currentPlayerCold = 0;
    }

    public void LockTotalCropCount()
    {
        totalCrops = CropCount;
        Crop.GrowGoalToday = totalCrops;
    }

    public IEnumerator DayReport()
    {
        OnEndOfDay?.Invoke();

        dayReportText.text = "Crops Remaining: " + CropCount + " / " + totalCrops;
        Crop.GrowGoalToday = CropCount;
        Crop.NumGrownToday = 0;

        yield return StartCoroutine(Utils.ChangeCanvasGroupAlpha(dayReportCV, 1, fadeUIRate));

        yield return new WaitUntil(() => Input.GetKey(closeDayResultScreenKey));

        yield return StartCoroutine(Utils.ChangeCanvasGroupAlpha(dayReportCV, 0, fadeUIRate));

        yield return StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(
            new List<AnimationActionSequenceEntry>() { new AnimationActionSequenceEntry("Sleep", null, null, 0, true, true) }));
    }

    public void SetDayData()
    {
        DayData data = dayDataMap[CurrentDay];

        // Weather
        WeatherManager._Instance.SetWeather(data.WeatherType);

        // Todo
        toDoList.Clear();
        toDoList.AddListItems(repeatedTasks);
        toDoList.AddListItems(data.ToDo);

        // Journal
        SetJournal();
    }

    public void CheckToDoItem(string key)
    {
        toDoList.CheckListItem(key);
    }

    public void SetJournal()
    {
        journal.ResetJournalForDay(CurrentDay + DayNightManager._Instance.StartDay);
    }

    public IEnumerator TooColdSequence()
    {
        p_Input.LockInput = true;
        BeingForcedInsideForBeingTooCold = true;
        ShowGameEventTriggerOpporotunity._Instance.Clear();

        yield return StartCoroutine(DialogueManager._Instance.ExecuteDialogue(tooColdDialogue));

        yield return new WaitForSeconds(delayAfterTooColdDialogue);

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Go Inside & Teleport to Bedside
            LoadLocationState(PlayerLocationState.INTERIOR, true);
        }, 1, false, true);
        animationSequence[1] = new AnimationActionSequenceEntry("Fade", null, null, 1, true, true);

        yield return StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));

        BeingForcedInsideForBeingTooCold = false;
        LockMovement = false;
    }

    public IEnumerator ForceSleepSequence()
    {
        p_Input.LockInput = true;
        BeingForcedToSleep = true;
        ShowGameEventTriggerOpporotunity._Instance.Clear();

        yield return StartCoroutine(DialogueManager._Instance.ExecuteDialogue(dayDataMap[CurrentDay].ForcedAsleepDialogue));

        yield return new WaitForSeconds(delayAfterForceSleepDialogue);

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Go Inside & Teleport to Bedside
            LoadLocationState(PlayerLocationState.BEDSIDE, true);
        }, 1, false, true);
        animationSequence[1] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Enter Bed as if had activated Trigger
            bedTrigger.Trigger(true);
        }, 1, true, true);
        yield return StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }

    public void BeginGame()
    {
        // Trigger Get Up from Seat
        GetUpFromStartSeatTrigger.Trigger();

        // Fade out then disable main menu screen
        StartCoroutine(Utils.ChangeCanvasGroupAlpha(mainMenuCV, 0, fadeUIRate, () => mainMenuScreen.SetActive(false)));

        // Fade in in game UI
        StartCoroutine(Utils.ChangeCanvasGroupAlpha(inGameUICV, 1, fadeUIRate));

        p_Input.LockInput = false;
        ShowGameEventTriggerOpporotunity._Instance.Enable = true;
        GameStarted = true;
    }

}
