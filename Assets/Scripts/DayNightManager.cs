using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DayNightManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField] private SerializableDictionary<TimeOfDayLabel, float> timeOfDayMap = new SerializableDictionary<TimeOfDayLabel, float>();
    [SerializeField] private TimeOfDayLabel startTimeOfDay;
    [SerializeField] private float timeScale;
    public TimeOfDayLabel CurrentTimeOfDayLabel { get; private set; }
    public float TimeOfDay { get; private set; }
    private float timeOfDayTimer;
    public bool EnableTimeFlow { get; set; }
    private float maxTimeOfDay = 24;
    public float PercentThroughDay => timeOfDayTimer / forceSleepAtTODTimerValue;

    private float nextTimeOfDayBreakpoint;
    private TimeOfDayLabel nextTimeOfDayLabel;

    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI timeOfDayText;

    [Header("Force Sleep")]
    [SerializeField] private float forceSleepAtTODTimerValue;
    private bool canForceSleep = true;

    [Header("Night Lights")]
    private NightLight[] nightLights;
    private GameObjectState currentNightLightsState;

    [Header("Calendar")]
    [SerializeField] private int startDay;
    public int StartDay => startDay;
    [SerializeField] private int endDay;
    public int EndDay => endDay;
    [SerializeField] private Calendar calendar;
    public bool HasCrossedOutToday { get; private set; }
    public static DayNightManager _Instance { get; private set; }

    [SerializeField] private ToDoList toDoList;

    public void ResetHasCrossedOutToday()
    {
        HasCrossedOutToday = false;
    }


    private void Awake()
    {
        _Instance = this;

        nightLights = FindObjectsOfType<NightLight>();

        CurrentTimeOfDayLabel = startTimeOfDay;
        CatchUpTimerToCurrentTimeOfDay();
        SetNextTimeOfDayBreakpoint();

        EnableTimeFlow = true;

        TimeOfDay = (timeOfDayTimer % maxTimeOfDay) / maxTimeOfDay;
        UpdateLighting(TimeOfDay);
    }

    public void CrossoutCurrentDay()
    {
        calendar.CrossoutDay(startDay + GameManager._Instance.CurrentDay);
        toDoList.CheckListItem("CrossoutDay");
        HasCrossedOutToday = true;
    }

    private void CatchUpTimerToCurrentTimeOfDay()
    {
        string[] todLabels = Enum.GetNames(typeof(TimeOfDayLabel));
        for (int i = 0; i < todLabels.Length; i++)
        {
            TimeOfDayLabel todLabel = (TimeOfDayLabel)i;
            timeOfDayTimer += timeOfDayMap[todLabel];
            nextTimeOfDayBreakpoint += timeOfDayMap[todLabel];
            if (todLabel == CurrentTimeOfDayLabel)
                break;
        }
    }

    private void SetNextTimeOfDayBreakpoint()
    {
        // Determine Time of Day
        if ((int)CurrentTimeOfDayLabel + 1 > Enum.GetNames(typeof(TimeOfDayLabel)).Length - 1)
        {
            nextTimeOfDayLabel = (TimeOfDayLabel)0;
        }
        else
        {
            nextTimeOfDayLabel = CurrentTimeOfDayLabel + 1;
        }
        nextTimeOfDayBreakpoint += timeOfDayMap[nextTimeOfDayLabel];
    }

    public void SetTimeOfDay(TimeOfDayLabel tod)
    {
        CurrentTimeOfDayLabel = tod;
        timeOfDayTimer = 0;
        nextTimeOfDayBreakpoint = 0;
        CatchUpTimerToCurrentTimeOfDay();
        SetNextTimeOfDayBreakpoint();
    }

    private void SetClockText()
    {
        // Get Integer part of timer
        int fp = Mathf.FloorToInt(timeOfDayTimer);
        int adjustedFp = fp;
        if (adjustedFp > 24)
        {
            adjustedFp -= 24;
        }
        string fpStr = adjustedFp.ToString();
        if (fpStr.Length == 1)
            fpStr = "0" + fpStr;

        // from decimal part of timer, convert to 60 rep
        float spd = (timeOfDayTimer - fp) * 60;
        int sp = Mathf.RoundToInt(spd);
        string spStr = sp.ToString();
        if (spStr.Length == 1)
            spStr = "0" + spStr;

        clockText.text = fpStr + ":" + spStr;
        timeOfDayText.text = CurrentTimeOfDayLabel.ToString();
    }

    private void Update()
    {
        SetClockText();

        if (preset == null) return;

        if (GameManager._Instance.BeingForcedInsideForBeingTooCold) return;

        if (EnableTimeFlow && GameManager._Instance.GameStarted)
        {
            timeOfDayTimer += Time.deltaTime * timeScale;
            TimeOfDay = (timeOfDayTimer % maxTimeOfDay) / maxTimeOfDay;

            if (timeOfDayTimer > nextTimeOfDayBreakpoint)
            {
                // Time of Day changed
                CurrentTimeOfDayLabel = nextTimeOfDayLabel;
                SetNextTimeOfDayBreakpoint();

                // Do stuff
                if (CurrentTimeOfDayLabel == TimeOfDayLabel.EVENING && currentNightLightsState == GameObjectState.INACTIVE)
                {
                    EnableNightLights();
                }
                if (CurrentTimeOfDayLabel == TimeOfDayLabel.MORNING && currentNightLightsState == GameObjectState.ACTIVE)
                {
                    DisableNightLights();
                }
            }

            if (CurrentTimeOfDayLabel == TimeOfDayLabel.MIDNIGHT && timeOfDayTimer >= forceSleepAtTODTimerValue && canForceSleep)
            {
                canForceSleep = false;
                EnableTimeFlow = false;
                StartCoroutine(GameManager._Instance.ForceSleepSequence());
            }
        }
        UpdateLighting(TimeOfDay);
    }

    private void EnableNightLights()
    {
        ToggleNightLights(GameObjectState.ACTIVE);
        currentNightLightsState = GameObjectState.ACTIVE;
    }

    private void DisableNightLights()
    {
        ToggleNightLights(GameObjectState.INACTIVE);
        currentNightLightsState = GameObjectState.INACTIVE;
    }

    private void ToggleNightLights(GameObjectState state)
    {
        foreach (NightLight light in nightLights)
        {
            light.ChangeState(state);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);
        RenderSettings.fogDensity = preset.FogDensity;

        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void OnValidate()
    {
        if (directionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }

    public void SetLightingPreset(LightingPreset preset)
    {
        this.preset = preset;
    }

    public void ResetCanForceSleep()
    {
        canForceSleep = true;
    }
}