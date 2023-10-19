using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct DayData
{
    public WeatherType WeatherType;
    public Dialogue[] forcedAsleepDialogue;
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
    private int currentDay;

    [SerializeField] private Transform playerTransform;
    public KeyCode PlayerInteractKey => KeyCode.E;

    public bool PlayerIsInside => isPlayerLocationStateInsideMap[CurrentLocationState];

    public static GameManager _Instance { get; private set; }

    public CameraFollowSubject ActiveCamera => playerLocationStateData[CurrentLocationState].Camera;
    public bool LockMovement { get; set; }
    public bool BeingForcedToSleep { get; private set; }
    public Action OnSleep { get; set; }

    [Header("References")]
    [SerializeField] private PlayerInput p_Input;
    [SerializeField] private GameEventTrigger bedTrigger;
    [SerializeField] private Light[] interiorLights;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float exteriorAmbienceVol;
    [SerializeField] private float interiorAmbienceVol;
    [SerializeField] private float exteriorLowpassFreq;
    [SerializeField] private float interiorLowpassFreq;

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
    }

    public void LoadLocationState(PlayerLocationState state)
    {
        CurrentLocationState = state;
        LoadState(CurrentLocationState);

        // Reset the Weather as there are some audio sources that will only play while inside
        WeatherManager._Instance.SetWeather(dayDataMap[currentDay].WeatherType);
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
        currentDay++;
        SetDayData();

        DayNightManager._Instance.ResetCanForceSleep();
        DayNightManager._Instance.EnableTimeFlow = true;
        BeingForcedToSleep = false;

        OnSleep?.Invoke();
    }

    public void SetDayData()
    {
        DayData data = dayDataMap[currentDay];
        WeatherManager._Instance.SetWeather(data.WeatherType);
    }

    public IEnumerator ForceSleepSequence()
    {
        p_Input.LockInput = true;
        BeingForcedToSleep = true;

        yield return StartCoroutine(DialogueManager._Instance.ExecuteDialogue(dayDataMap[currentDay].forcedAsleepDialogue));

        yield return new WaitForSeconds(1);

        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Go Inside & Teleport to Bedside
            LoadLocationState(PlayerLocationState.BEDSIDE);
        }, 1, false, true);
        animationSequence[1] = new AnimationActionSequenceEntry("Fade", null, delegate
        {
            // Enter Bed as if had activated Trigger
            bedTrigger.Trigger();
        }, 1, true, true);
        yield return StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }
}
