using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherType
{
    CLEAR,
    SUNNY,
    WINDY,
    RAINY,
    STORMY
}

[System.Serializable]
public struct WeatherTypeData
{
    public GameObject[] EnableForWeather;
    public LightingPreset LightingPreset;
    public ContinuousAudioSource[] AmbientAudioSources;
    public ContinuousAudioSource[] InteriorExplicitAmbientAudioSources;
}

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager _Instance { get; private set; }

    [SerializeField] private SerializableDictionary<WeatherType, WeatherTypeData> weatherTypeData = new SerializableDictionary<WeatherType, WeatherTypeData>();
    private List<GameObject> enabledGameObjects = new List<GameObject>();
    private List<ContinuousAudioSource> enabledAudioSources = new List<ContinuousAudioSource>();
    public WeatherType CurrentWeatherType { get; private set; }

    [Header("Flashes")]
    [SerializeField] private Vector2 minMaxTimeBetweenFlashes = new Vector2(15, 30);
    [SerializeField] private Vector2 minMaxAllowedSpawningPosX = new Vector2(-100, 100);
    [SerializeField] private Vector2 minMaxAllowedSpawningPosY = new Vector2(50, 100);
    [SerializeField] private Vector2 minMaxAllowedSpawningPosZ = new Vector2(-100, 100);
    [SerializeField] private Vector2 chanceToTriggerMultiple = new Vector2(1, 10);
    [SerializeField] private Vector2 chanceToRecurse = new Vector2(10, 25);
    [SerializeField] private Vector2 minMaxTimeBetweenRecursingActivations = new Vector2(.5f, 1.5f);
    [SerializeField] private float reduceChanceToRecursePerRecurse = 2;
    [SerializeField] private FlashOfLight flashOfLightPrefab;
    private Coroutine spawnFlashesCoroutine;

    private void Awake()
    {
        _Instance = this;
    }

    private IEnumerator SpawnFlashes()
    {
        float timeBetweenFlashes = RandomHelper.RandomFloat(minMaxTimeBetweenFlashes);
        yield return new WaitForSeconds(timeBetweenFlashes);
        SpawnFlash(chanceToRecurse, 0);
        spawnFlashesCoroutine = StartCoroutine(SpawnFlashes());
    }

    private void SpawnFlash(Vector2 chanceToRecurse, float timeToActivate)
    {
        if (chanceToRecurse.x <= 0) return;

        Vector3 spawnPos = new Vector3(
            RandomHelper.RandomFloat(minMaxAllowedSpawningPosX),
            RandomHelper.RandomFloat(minMaxAllowedSpawningPosY),
            RandomHelper.RandomFloat(minMaxAllowedSpawningPosZ));
        FlashOfLight light = Instantiate(flashOfLightPrefab, spawnPos, Quaternion.identity);
        light.Activate(timeToActivate);

        if (RandomHelper.EvaluateChanceTo(chanceToTriggerMultiple)
            && RandomHelper.EvaluateChanceTo(chanceToRecurse))
        {
            chanceToRecurse.x -= reduceChanceToRecursePerRecurse;
            SpawnFlash(chanceToRecurse, timeToActivate + RandomHelper.RandomFloat(minMaxTimeBetweenRecursingActivations));
        }
    }

    public void SetWeather(WeatherType type)
    {
        // Spawn Flashes During Stormy Weather
        if (type != WeatherType.STORMY && spawnFlashesCoroutine != null)
        {
            // New weather is anything other than stormy, stop the storming flashes
            StopCoroutine(spawnFlashesCoroutine);
        }
        else if (CurrentWeatherType != WeatherType.STORMY && type == WeatherType.STORMY)
        {
            // New weather is Stormy and previous weather was not already stormy
            // Ensures that we don't start multiple of the coroutines
            spawnFlashesCoroutine = StartCoroutine(SpawnFlashes());
        }

        CurrentWeatherType = type;
        WeatherTypeData data = weatherTypeData[type];


        // Ambience
        // Disable previous objects
        foreach (GameObject obj in enabledGameObjects)
        {
            obj.SetActive(false);
        }

        // Enable new objects
        foreach (GameObject obj in data.EnableForWeather)
        {
            obj.SetActive(true);
            enabledGameObjects.Add(obj);
        }

        // Ambience
        // Disable previous sources
        foreach (ContinuousAudioSource source in enabledAudioSources)
        {
            source.Muted = true;
        }

        // Enable new sources
        foreach (ContinuousAudioSource source in data.AmbientAudioSources)
        {
            source.Muted = false;
            enabledAudioSources.Add(source);
        }

        if (GameManager._Instance.PlayerIsInside)
        {
            // Enable new sources
            foreach (ContinuousAudioSource source in data.InteriorExplicitAmbientAudioSources)
            {
                source.Muted = false;
                enabledAudioSources.Add(source);
            }
        }

        DayNightManager._Instance.SetLightingPreset(data.LightingPreset);
    }
}
