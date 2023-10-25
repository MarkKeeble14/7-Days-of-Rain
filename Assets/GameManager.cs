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
    public Dialogue[] TendedToAllCropsDialogue;
    public ToDoListItemData[] ToDo;
}

public enum PlayerLocationState
{
    SPAWN,
    EXTERIOR,
    INTERIOR,
    BEDSIDE,
    IN_BED
}

[System.Serializable]
public struct PlayerLocationStateEntryData
{
    public SerializableDictionary<GameObjectState, GameObject[]> ObjectStateChanges;
    public Vector3 PlayerPosition;
    public Vector3 PlayerEulerAngles;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerLocationState currentLocationState;
    public PlayerLocationState CurrentLocationState
    {
        get
        {
            return currentLocationState;
        }
        set
        {
            currentLocationState = value;
        }
    }
    [SerializeField] private SerializableDictionary<PlayerLocationState, PlayerLocationStateEntryData> playerLocationStateData;
    [SerializeField] private SerializableDictionary<PlayerLocationState, bool> isPlayerLocationStateInsideMap = new SerializableDictionary<PlayerLocationState, bool>();
    [SerializeField] private DayData[] dayDataMap = new DayData[7];
    public bool PlayerInControl => CameraController.PlayerIsSubject;
    public int CurrentDay { get; set; }
    public bool PlayerIsInside => isPlayerLocationStateInsideMap[CurrentLocationState];
    public static GameManager _Instance { get; private set; }
    [SerializeField] private CameraFollowSubject cameraController;
    public CameraFollowSubject CameraController => cameraController;
    public bool LockMovement { get; set; }
    public bool GameStarted { get; private set; }
    [SerializeField] private AlterTransformGameEvent resetOutsideCameraGameEvent;
    [SerializeField] private SerializableDictionary<int, GameObject[]> destroyObjectsOnEndOfDayMap = new SerializableDictionary<int, GameObject[]>();
    [SerializeField] private SerializableDictionary<int, GameObject[]> enableObjectsOnEndOfDayMap = new SerializableDictionary<int, GameObject[]>();

    [Header("Sleep")]
    [SerializeField] private float delayAfterForceSleepDialogue = 2;
    [SerializeField] private GameEventTriggerOnKeyPress getUpFromBedTrigger;
    public bool BeingForcedToSleep { get; private set; }
    public Action OnEndOfDay { get; set; }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerInput p_Input;
    [SerializeField] private GameEventTrigger bedTrigger;
    [SerializeField] private Light[] interiorLights;
    [SerializeField] private Light[] interiorToggleableLights;
    [SerializeField] private PlayAudioGameEvent doorCloseSoundGameEvent;

    [Header("Cold")]
    [SerializeField] private Image coldBar;
    [SerializeField] private float accumulateColdRate;
    [SerializeField] private float warmUpRate;
    [SerializeField] private float maxColdValue;
    [SerializeField] private Dialogue[] tooColdDialogue;
    [SerializeField] private float delayAfterTooColdDialogue = 2;
    [SerializeField] private float exposedToRainColdModifier = 1.15f;
    [SerializeField] private SimpleAudioClipContainer tooColdAudio;
    [SerializeField] private float warmthPerInteriorLight = 0.1f;
    private Coroutine tooColdSequence;
    public bool PlayerExposedToRain
    {
        get
        {
            RaycastHit hit;
            return !Physics.Raycast(playerTransform.position, Vector2.up, out hit);
        }
    }
    [SerializeField] private SerializableDictionary<TimeOfDayLabel, float> timeOfDayColdModifier = new SerializableDictionary<TimeOfDayLabel, float>();
    [SerializeField] private TimeOfDayLabel nightTimeActiivitiesBeginAt = TimeOfDayLabel.NIGHT;
    private float currentPlayerCold;
    public float PercentColdValue => currentPlayerCold / maxColdValue;
    public bool BeingForcedInsideForBeingTooCold { get; private set; }
    public bool InGameUIActive { get; set; }

    [Header("Start")]
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private CanvasGroup mainMenuCV;
    [SerializeField] private float fadeUIRate;
    [SerializeField] private CanvasGroup inGameUICV;
    [SerializeField] private GameEvent getUpFromBedGameEvent;

    [Header("End of Game")]
    [SerializeField] private CanvasGroup endOfGameCV;
    [SerializeField] private JournalEntryDisplay journalEntryPrefab;
    [SerializeField] private float delayBetweenJournalEntrySpawns;
    [SerializeField] private Transform journalEntryList;
    [SerializeField] private TextMeshProUGUI cropsResultText;
    [SerializeField] private TextMeshProUGUI endingText;

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
    public float SatedSpeedModifier { get; private set; }
    [SerializeField] private float missedBreakfastSpeedPenalty;
    [SerializeField] private float missedLunchSpeedPenalty;
    [SerializeField] private float missedDinnerSpeedPenalty;

    [SerializeField] private Transform dayReportToDoListModsParent;
    [SerializeField] private TextMeshProUGUI dayReportTextPrefab;
    private List<TextMeshProUGUI> spawnedAdditionalDayReportTexts = new List<TextMeshProUGUI>();

    [Header("Journal")]
    [SerializeField] private Journal journal;

    [Header("Crop")]
    [SerializeField] private float maxDirtCapacity;
    public float CurrentDirtPercentage => CurrentDirtAmount / maxDirtCapacity;
    [SerializeField] private Image dirtAmount;
    [SerializeField] private GameObject dirtAmountContainer;
    [SerializeField, Range(0, 1)] private float percentCropHarvestedDensityForGoodEnding = 0.9f;
    public bool TendedToAllCropsToday { get; private set; }
    public float CurrentDirtAmount { get; private set; }

    [Header("Coyote")]
    [SerializeField] private SerializableDictionary<int, bool> enableCoyoteSoundsOnDays = new SerializableDictionary<int, bool>();
    [SerializeField] private SimpleAudioClipContainer coyoteSequenceBegunSound;
    [SerializeField] private RandomClipAudioClipContainer ambientCoyoteSounds;
    [SerializeField] private Vector2 minMaxTimeTillNextCoyoteSequence;
    [SerializeField] private Vector2 minMaxTimeBetweenCoyoteAmbientSounds;
    [SerializeField] private float inCoyoteSequenceAmbientTimerMod = 3;
    [SerializeField] private Dialogue[] firstCoyoteDialogue;
    private float coyoteAmbientTimer;
    private float nextCoyoteSequenceTimer;
    private bool inCoyoteSequence;
    private bool hasPlayedCoyoteDialogue;

    [Header("Monster Chase")]
    [SerializeField] private MonsterMovement monsterMovement;
    [SerializeField] private SimpleAudioClipContainer monsterChaseOn;
    [SerializeField] private int canInitiateMonsterChaseBeginningDayNo = 1;
    [SerializeField] private Vector2 minMaxTimeTillNextMonsterSequence;
    private float nextMonsterSequenceTimer;
    private bool inMonsterSequence;
    public bool InMonsterSequence => inMonsterSequence;

    [Header("Spooky Wind")]
    [SerializeField] private FadeAudioSource spookyWindSource;
    private bool spookyWindEnabled;

    [Header("Heartbeats")]
    [SerializeField] private ContinuousAudioSource heartBeats;

    [Header("Monster Massacre")]
    [SerializeField] private SimpleAudioClipContainer monsterMassacre;
    [SerializeField] private float monsterMassacreWaitTime;
    [SerializeField] private GameObject[] coyoteBodies;
    [SerializeField] private Dialogue[] monsterMassacreDialogue;
    [SerializeField] private LightingPreset monsterMassacreLightingPreset;

    [Header("Kill Animals")]
    [SerializeField] private Vector2 minMaxTimeBetweenAnimalDeaths;
    [SerializeField] private GameObject chickenPenGate;
    [SerializeField] private GameObject cowPenGate;

    [Header("Enemy Murder")]
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private float timeBeforeEnemyMunchPlays = 1;
    [SerializeField] private CameraFollowSubjectData enemyMurderSubject;
    [SerializeField] private SimpleAudioClipContainer enemyScream;
    [SerializeField] private float timeBeforeEnemyScreamPlays;
    [SerializeField] private SimpleAudioClipContainer monsterMunching;
    [SerializeField] private float durationOfBlackScreenAfterKill = 1.25f;

    private bool hasPlayedPostFirstEncounterDialogue;
    [SerializeField] private Dialogue[] firstEncounterDialogue;

    public void BeginCoyoteSequence()
    {
        inCoyoteSequence = true;
        CanSprintDisplayController._Instance.CanSprint = true;
        coyoteSequenceBegunSound.PlayOneShot();
    }

    public void EndCoyoteSequence()
    {
        if (!hasPlayedPostFirstEncounterDialogue && inCoyoteSequence)
        {
            hasPlayedPostFirstEncounterDialogue = true;
            StartCoroutine(DialogueManager._Instance.ExecuteDialogue(firstEncounterDialogue));
        }

        inCoyoteSequence = false;
        CanSprintDisplayController._Instance.CanSprint = false;
        nextCoyoteSequenceTimer = RandomHelper.RandomFloat(minMaxTimeTillNextCoyoteSequence);
    }

    [ContextMenu("Begin Monster")]
    public void BeginMonsterSequence()
    {
        inMonsterSequence = true;
        monsterMovement.Wake();
        CanSprintDisplayController._Instance.CanSprint = true;
        monsterChaseOn.PlayOneShot();
    }

    [ContextMenu("End Monster")]
    public void EndMonsterSequence()
    {
        if (!hasPlayedPostFirstEncounterDialogue && inMonsterSequence)
        {
            hasPlayedPostFirstEncounterDialogue = true;
            StartCoroutine(DialogueManager._Instance.ExecuteDialogue(firstEncounterDialogue));
        }

        inMonsterSequence = false;
        monsterMovement.Sleep();
        CanSprintDisplayController._Instance.CanSprint = false;
        nextMonsterSequenceTimer = RandomHelper.RandomFloat(minMaxTimeTillNextMonsterSequence);
    }

    public void AlterDirt(float alterBy)
    {
        CurrentDirtAmount += alterBy;
        if (CurrentDirtAmount < 0) CurrentDirtAmount = 0;
        else if (CurrentDirtAmount > maxDirtCapacity) CurrentDirtAmount = maxDirtCapacity;

        dirtAmount.fillAmount = CurrentDirtPercentage;
    }

    public IEnumerator SetStateOfInteriorLights(GameObjectState state)
    {
        foreach (Light light in interiorLights)
        {
            light.enabled = state == GameObjectState.ACTIVE ? true : false;
            yield return null;
        }
    }

    public void AlterCold(float alterBy)
    {
        currentPlayerCold += alterBy;
        if (currentPlayerCold > maxColdValue) currentPlayerCold = maxColdValue;
        if (currentPlayerCold < 0) currentPlayerCold = 0;
    }

    private void Awake()
    {
        _Instance = this;

        SatedSpeedModifier = 1;
    }

    private void Start()
    {
        p_Input.LockInput = true;
        LockMovement = true;
        SetDayData();
        LoadState(CurrentLocationState);
        ShowGameEventTriggerOpporotunity._Instance.Enable = false;
        journal.ResetJournalForDay(CurrentDay + 1 + DayNightManager._Instance.StartDay);

        EndCoyoteSequence();
        EndMonsterSequence();
    }

    private void Update()
    {
        if (!GameStarted) return;

        // Heartbeats
        heartBeats.Muted = !inMonsterSequence && !inCoyoteSequence && GameStarted;

        // Night time activities
        if (DayNightManager._Instance.CurrentTimeOfDayLabel >= nightTimeActiivitiesBeginAt)
        {
            // Spooky Wind
            if (!spookyWindEnabled)
            {
                spookyWindSource.CallFade(1);
                spookyWindEnabled = true;
            }

            if (!BeingForcedInsideForBeingTooCold && !BeingForcedToSleep)
            {
                // Monster
                if (CurrentDay >= canInitiateMonsterChaseBeginningDayNo)
                {
                    if (!PlayerIsInside && !inMonsterSequence)
                    {
                        if (nextMonsterSequenceTimer <= 0)
                        {
                            BeginMonsterSequence();
                        }
                        else
                        {
                            nextMonsterSequenceTimer -= Time.deltaTime;
                        }
                    }
                }

                // Coyotes
                if (enableCoyoteSoundsOnDays[CurrentDay])
                {
                    // Audio
                    coyoteAmbientTimer -= Time.deltaTime * (inCoyoteSequence ? inCoyoteSequenceAmbientTimerMod : 1);
                    if (coyoteAmbientTimer <= 0)
                    {
                        ambientCoyoteSounds.PlayOneShot();
                        coyoteAmbientTimer = RandomHelper.RandomFloat(minMaxTimeBetweenCoyoteAmbientSounds);

                        if (!hasPlayedCoyoteDialogue)
                        {
                            hasPlayedCoyoteDialogue = true;
                            StartCoroutine(DialogueManager._Instance.ExecuteDialogue(firstCoyoteDialogue));
                        }
                    }

                    // Sequence
                    if (!PlayerIsInside && !inCoyoteSequence)
                    {
                        if (nextCoyoteSequenceTimer <= 0)
                        {
                            BeginCoyoteSequence();
                        }
                        else
                        {
                            nextCoyoteSequenceTimer -= Time.deltaTime;
                        }
                    }
                }
            }
        }

        // Fade in in game UI
        inGameUICV.alpha = Mathf.MoveTowards(inGameUICV.alpha, InGameUIActive ? 1 : 0, fadeUIRate * Time.deltaTime);

        if (BeingForcedInsideForBeingTooCold || BeingForcedToSleep)
        {
            dirtAmountContainer.SetActive(false);
            return;
        };

        if (!PlayerIsInside && currentPlayerCold < maxColdValue)
        {
            currentPlayerCold += Time.deltaTime * (accumulateColdRate + WeatherManager._Instance.CurrentWeatherColdGainRateIncrease)
                * (PlayerExposedToRain ? exposedToRainColdModifier : 1)
                * timeOfDayColdModifier[DayNightManager._Instance.CurrentTimeOfDayLabel];
            if (currentPlayerCold > maxColdValue)
            {
                StartTooColdSequence();
                currentPlayerCold = maxColdValue;
            }
        }
        else if (PlayerIsInside && currentPlayerCold > 0)
        {
            currentPlayerCold -= Time.deltaTime * warmUpRate;

            float interiorLightWarmth = 0;
            foreach (Light l in interiorToggleableLights)
            {
                if (l.enabled) interiorLightWarmth += warmthPerInteriorLight;
            }
            currentPlayerCold -= interiorLightWarmth;

            if (currentPlayerCold < 0) currentPlayerCold = 0;
        }

        coldBar.fillAmount = PercentColdValue;
        dirtAmount.fillAmount = CurrentDirtPercentage;
        dirtAmountContainer.SetActive(!PlayerIsInside);
    }

    public void LoadLocationState(PlayerLocationState state, PlayAudioGameEvent accompanyingAudio)
    {
        if (accompanyingAudio != null &&
            ((isPlayerLocationStateInsideMap[CurrentLocationState] && !isPlayerLocationStateInsideMap[state])
            || (!isPlayerLocationStateInsideMap[CurrentLocationState] && isPlayerLocationStateInsideMap[state])))
        {
            accompanyingAudio.CallActivate();
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

        // Enable/Disable Objects
        EnableForGivenState(data);
        SetMixerState();
    }

    private void SetMixerState()
    {
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

    [ContextMenu("Test")]
    public void Test()
    {
        StartCoroutine(MonsterMassacreSequence());
    }

    private IEnumerator KillAnimalsSequence()
    {
        chickenPenGate.SetActive(false);
        cowPenGate.SetActive(false);

        // Kill Animals
        AnimalData[] animals = GameObject.FindObjectsOfType<AnimalData>();
        foreach (AnimalData data in animals)
        {
            data.Kill();
            yield return new WaitForSeconds(RandomHelper.RandomFloat(minMaxTimeBetweenAnimalDeaths));
        }
    }

    private void NewDay()
    {
        // Destroy old objects
        foreach (GameObject obj in destroyObjectsOnEndOfDayMap[CurrentDay])
        {
            Destroy(obj);
        }
        CurrentDay++;
        // Enable new objects
        foreach (GameObject obj in enableObjectsOnEndOfDayMap[CurrentDay])
        {
            obj.SetActive(true);
        }

        DayNightManager._Instance.ResetCanForceSleep();
        DayNightManager._Instance.ResetHasCrossedOutToday();
        DayNightManager._Instance.SetTimeOfDay(TimeOfDayLabel.MORNING);
        BeingForcedToSleep = false;
        BeingForcedInsideForBeingTooCold = false;

        currentPlayerCold = 0;
        AlterDirt(-CurrentDirtAmount);
        TendedToAllCropsToday = false;

        spookyWindEnabled = false;
        spookyWindSource.SetVolume(0);
    }


    public void SaveJournalEntry()
    {
        journal.SaveJournalEntry(CurrentDay + 1 + DayNightManager._Instance.StartDay);
    }

    public IEnumerator Sleep()
    {
        EndCoyoteSequence();
        EndMonsterSequence();

        // Journal
        SaveJournalEntry();
        journal.ResetJournalForDay(CurrentDay + 1 + DayNightManager._Instance.StartDay);

        // Sleepy Time Events
        switch (CurrentDay)
        {
            case 0:
                yield return StartCoroutine(MonsterMassacreSequence());
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                // Last day allowed
                RunEnding();
                yield break;
            default:
                break;
        }

        NewDay();
        yield return StartCoroutine(DayReport());
    }

    private IEnumerator MonsterMassacreSequence()
    {
        TransitionManager._Instance.SetAnimationToState("Eyelids", TransitionDirection.OUT);
        TransitionManager._Instance.SetAnimationToState("Sleep", TransitionDirection.IN);

        yield return new WaitForSeconds(1);

        DayNightManager._Instance.OverrideLightingPreset(monsterMassacreLightingPreset);

        // Begin to play sound
        monsterMassacre.PlayOneShot();

        yield return new WaitForSeconds(1);

        StartCoroutine(DialogueManager._Instance.ExecuteDialogue(monsterMassacreDialogue));

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        // Open Eyes
        animationSequence[0] = new AnimationActionSequenceEntry("Eyelids", () => StartCoroutine(KillAnimalsSequence()), null, monsterMassacreWaitTime, true, true);
        // Stare for some time
        // Close Eyes
        animationSequence[1] = new AnimationActionSequenceEntry("Eyelids", null, null, 1, false, true);
        yield return StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));

        // Enable Coyote Body
        foreach (GameObject obj in coyoteBodies)
        {
            obj.SetActive(true);
        }

        DayNightManager._Instance.StopOverrideLightingPreset();

        // Done
        TransitionManager._Instance.SetAnimationToState("Sleep", TransitionDirection.OUT);
        TransitionManager._Instance.SetAnimationToState("Eyelids", TransitionDirection.IN);
    }

    public void LockTotalCropCount()
    {
        Crop.GrowGoalToday = Crop.NumSpawned;
    }

    public void LastCropHarvested()
    {
        RunEnding();
    }

    public IEnumerator DayReport()
    {
        OnEndOfDay?.Invoke();

        Crop.GrowGoalToday = (Crop.NumSpawned - Crop.NumHarvested - Crop.NumDead);
        dayReportText.text = "Crops Remaining: " + Crop.GrowGoalToday + " / " + Crop.NumSpawned
            + "\nCrops Harvested: " + Crop.NumHarvested + " / " + Crop.NumSpawned;
        Crop.NumGrownToday = 0;

        // To Do List
        // Speed Modifier
        SatedSpeedModifier = 1;
        if (!toDoList.IsItemChecked("Breakfast"))
        {
            SatedSpeedModifier -= missedBreakfastSpeedPenalty;
            TextMeshProUGUI text = Instantiate(dayReportTextPrefab, dayReportToDoListModsParent);
            text.text = "Missed Breakfast: Speed Lowered";
            spawnedAdditionalDayReportTexts.Add(text);
        }

        if (!toDoList.IsItemChecked("Lunch"))
        {
            SatedSpeedModifier -= missedLunchSpeedPenalty;
            TextMeshProUGUI text = Instantiate(dayReportTextPrefab, dayReportToDoListModsParent);
            text.text = "Missed Lunch: Speed Lowered";
            spawnedAdditionalDayReportTexts.Add(text);
        }
        if (!toDoList.IsItemChecked("Dinner"))
        {
            SatedSpeedModifier -= missedDinnerSpeedPenalty;
            TextMeshProUGUI text = Instantiate(dayReportTextPrefab, dayReportToDoListModsParent);
            text.text = "Missed Dinner: Speed Lowered";
            spawnedAdditionalDayReportTexts.Add(text);
        }

        SetDayData();

        yield return StartCoroutine(Utils.ChangeCanvasGroupAlpha(dayReportCV, 1, fadeUIRate));

        yield return new WaitUntil(() => Input.GetKey(closeDayResultScreenKey));

        yield return StartCoroutine(Utils.ChangeCanvasGroupAlpha(dayReportCV, 0, fadeUIRate));

        // Remove Old
        while (spawnedAdditionalDayReportTexts.Count > 0)
        {
            TextMeshProUGUI t = spawnedAdditionalDayReportTexts[0];
            Destroy(t.gameObject);
            spawnedAdditionalDayReportTexts.Remove(t);
        }

        ShowGameEventTriggerOpporotunity._Instance.TryAddTrigger(getUpFromBedTrigger);
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
    }

    public void TendedToAllCrops()
    {
        TendedToAllCropsToday = true;
        StartCoroutine(DialogueManager._Instance.ExecuteDialogue(dayDataMap[CurrentDay].TendedToAllCropsDialogue));
    }

    public void CheckToDoItem(string key)
    {
        toDoList.CheckListItem(key);
    }

    public void StartTooColdSequence()
    {
        tooColdSequence = StartCoroutine(TooColdSequence());
    }

    public IEnumerator TooColdSequence()
    {
        EndCoyoteSequence();
        EndMonsterSequence();

        p_Input.LockInput = true;
        BeingForcedInsideForBeingTooCold = true;
        ShowGameEventTriggerOpporotunity._Instance.Clear();

        tooColdAudio.PlayOneShot();

        yield return StartCoroutine(DialogueManager._Instance.ExecuteDialogue(tooColdDialogue));

        yield return new WaitForSeconds(delayAfterTooColdDialogue);

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Go Inside & Teleport to Bedside
            LoadLocationState(PlayerLocationState.INTERIOR, doorCloseSoundGameEvent);
            resetOutsideCameraGameEvent.CallActivate();
        }, 1, false, true);
        animationSequence[1] = new AnimationActionSequenceEntry("Fade", null, null, 1, true, true);

        yield return StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));

        BeingForcedInsideForBeingTooCold = false;
        LockMovement = false;
        tooColdSequence = null;
    }

    public IEnumerator ForceSleepSequence()
    {
        EndCoyoteSequence();
        EndMonsterSequence();

        p_Input.LockInput = true;
        BeingForcedToSleep = true;
        ShowGameEventTriggerOpporotunity._Instance.Clear();

        yield return StartCoroutine(DialogueManager._Instance.ExecuteDialogue(dayDataMap[CurrentDay].ForcedAsleepDialogue));

        yield return new WaitForSeconds(delayAfterForceSleepDialogue);

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Go Inside & Teleport to Bedside
            LoadLocationState(PlayerLocationState.BEDSIDE, null);
            resetOutsideCameraGameEvent.CallActivate();
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
        getUpFromBedGameEvent.CallActivate();

        // Fade out then disable main menu screen
        StartCoroutine(Utils.ChangeCanvasGroupAlpha(mainMenuCV, 0, fadeUIRate, () => mainMenuScreen.SetActive(false)));

        InGameUIActive = true;

        p_Input.LockInput = false;
        ShowGameEventTriggerOpporotunity._Instance.Enable = true;
        GameStarted = true;
    }

    private void RunEnding()
    {
        float result = (float)Crop.NumHarvested / Crop.NumSpawned;
        if (result >= percentCropHarvestedDensityForGoodEnding)
        {
            GoodEnding();
        }
        else
        {
            BadEnding();
        }
    }

    [ContextMenu("Dead Ending")]
    public void DeadEnding()
    {
        endingText.text = "You Died\nEnding 1/3";
        OnEnding();

        // Stop too Cold Sequence if it is happening
        if (tooColdSequence != null)
        {
            StopCoroutine(tooColdSequence);
        }

        StartCoroutine(EnemyMurderSequence());
    }

    [ContextMenu("Bad Ending")]
    public void BadEnding()
    {
        endingText.text = "A Lacklaster Harvest\nEnding 2/3";
        OnEnding();

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            StartCoroutine(EndOfGameSequence());
        }, 1, false, true);
        TransitionManager._Instance.StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }


    [ContextMenu("Good Ending")]
    public void GoodEnding()
    {
        endingText.text = "A Successful Harvest\nEnding 3/3";
        OnEnding();

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            StartCoroutine(EndOfGameSequence());
        }, 1, false, true);
        TransitionManager._Instance.StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }

    private void OnEnding()
    {
        GameStarted = false;
        InGameUIActive = false;
        p_Input.LockInput = true;
        LockMovement = true;
        DayNightManager._Instance.EnableTimeFlow = false;
    }

    private IEnumerator EnemyMurderSequence()
    {
        TransitionManager._Instance.Interrupt();

        enemyAnim.SetTrigger("Scream");
        CameraController.SetNewSubject(enemyMurderSubject);

        yield return new WaitForSeconds(timeBeforeEnemyScreamPlays);
        enemyScream.PlayOneShot();

        yield return new WaitForSeconds(timeBeforeEnemyMunchPlays);
        monsterMunching.PlayOneShot();

        heartBeats.Muted = true;

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            StartCoroutine(EndOfGameSequence());
        }, 1, false, true);
        yield return TransitionManager._Instance.StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }

    private IEnumerator EndOfGameSequence()
    {
        yield return new WaitForSeconds(durationOfBlackScreenAfterKill);

        List<JournalEntry> journalEntries = journal.GetJournalEntries();
        foreach (JournalEntry entry in journalEntries)
        {
            JournalEntryDisplay spawned = Instantiate(journalEntryPrefab, journalEntryList);
            spawned.Set(entry);
            yield return new WaitForSeconds(delayBetweenJournalEntrySpawns);
        }

        cropsResultText.text = "Crops Harvested: " + Crop.NumHarvested + " / " + Crop.NumSpawned;

        yield return StartCoroutine(Utils.ChangeCanvasGroupAlpha(endOfGameCV, 1, fadeUIRate, null));

        endOfGameCV.blocksRaycasts = true;
        endOfGameCV.interactable = true;
    }
}
