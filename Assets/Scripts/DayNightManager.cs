using System;
using System.Collections;
using UnityEngine;

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

    [Header("Force Sleep")]
    [SerializeField] private float forceSleepAtTODTimerValue;
    private bool canForceSleep = true;

    [Header("Night Lights")]
    private NightLight[] nightLights;
    private GameObjectState currentNightLightsState;

    public static DayNightManager _Instance { get; private set; }

    private void Awake()
    {
        _Instance = this;

        nightLights = FindObjectsOfType<NightLight>();

        CurrentTimeOfDayLabel = startTimeOfDay;
        CatchUpTimerToCurrentTimeOfDay();
        SetNextTimeOfDayBreakpoint();

        EnableTimeFlow = true;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) SetTimeOfDay(TimeOfDayLabel.EVENING);

        if (preset == null) return;

        if (EnableTimeFlow)
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