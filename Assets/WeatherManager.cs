using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherType
{
    CLEAR,
    SUNNY,
    WINDY,
    RAINY,
    STORMY,
    APOCALYPTIC
}

[System.Serializable]
public struct LightningFlashData
{
    public bool Enable;
    public Vector2 MinMaxTimeBetweenFlashes;
    public Vector2 MinMaxAllowedSpawningPosX;
    public Vector2 MinMaxAllowedSpawningPosY;
    public Vector2 MinMaxAllowedSpawningPosZ;
}

[System.Serializable]
public struct SpookerFlashData
{
    public bool Enable;
    public Vector2 MinMaxDistanceFromPlayer;
    public Vector2 ChanceToShowSpookerWithFlash;
    public Vector2 MinMaxShownByFlashSpookerFadeSpeed;
    public Vector2 MinMaxShownByFlashSpookerDelayBetween;
    public Vector2 MinMaxTransparencyGoal;
    public Vector2 ShownByFlashSpookerDelay;
    public Vector2 MinMaxAllowedSpawningPosX;
    public Vector2 MinMaxAllowedSpawningPosZ;
    public TimeOfDayLabel BeginAt;
}

[System.Serializable]
public struct WeatherTypeData
{
    public GameObject[] EnableForWeather;
    public LightingPreset LightingPreset;
    public ContinuousAudioSource[] AmbientAudioSources;
    public ContinuousAudioSource[] InteriorExplicitAmbientAudioSources;
    public float ColdModifier;
    public LightningFlashData LightningFlashData;
    public SpookerFlashData SpookerFlashData;
}

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager _Instance { get; private set; }

    [SerializeField] private SerializableDictionary<WeatherType, WeatherTypeData> weatherTypeData = new SerializableDictionary<WeatherType, WeatherTypeData>();
    private List<GameObject> enabledGameObjects = new List<GameObject>();
    private List<ContinuousAudioSource> enabledAudioSources = new List<ContinuousAudioSource>();
    public WeatherType CurrentWeatherType { get; private set; }

    [Header("Flashes")]
    [SerializeField] private FlashOfLight flashOfLightPrefab;
    private Coroutine spawnFlashesCoroutine;
    private LightningFlashData currentLightningFlashData => weatherTypeData[CurrentWeatherType].LightningFlashData;
    private SpookerFlashData currentSpookerFlashData => weatherTypeData[CurrentWeatherType].SpookerFlashData;
    public float CurrentWeatherColdGainRateIncrease => weatherTypeData[CurrentWeatherType].ColdModifier;

    [Header("Shown by Flash Spooker")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform shownByFlashSpooker;
    [SerializeField] private SkinnedMeshRenderer shownByFlashSpookerRenderer;
    [SerializeField] private BindToLayer jumpscareSpookerBindToGround;
    private Material jumpScareSpookerMat;

    private void Awake()
    {
        _Instance = this;

        jumpScareSpookerMat = shownByFlashSpookerRenderer.sharedMaterial;
        Color c = jumpScareSpookerMat.color;
        c.a = 0;
        jumpScareSpookerMat.color = c;
    }

    [ContextMenu("Fade Spooker")]
    public void FadeSpooker()
    {
        StartCoroutine(FadeInThenOutMaterialTransparency(jumpScareSpookerMat,
            RandomHelper.RandomFloat(currentSpookerFlashData.MinMaxTransparencyGoal),
            RandomHelper.RandomFloat(currentSpookerFlashData.MinMaxShownByFlashSpookerFadeSpeed),
            RandomHelper.RandomFloat(currentSpookerFlashData.MinMaxShownByFlashSpookerDelayBetween),
            null));
    }

    private IEnumerator FadeInThenOutMaterialTransparency(Material mat, float transparencyGoal, float fadeSpeed, float delayBetween, Action onEnd)
    {
        Color startColor = mat.color;
        Color targetColor = mat.color;
        targetColor.a = transparencyGoal;
        Color newColor;
        while (mat.color.a < targetColor.a)
        {
            newColor = mat.color;
            newColor.a += Time.deltaTime * fadeSpeed;
            mat.color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(delayBetween);
        while (mat.color.a > 0)
        {
            newColor = mat.color;
            newColor.a -= Time.deltaTime * fadeSpeed;
            mat.color = newColor;
            yield return null;
        }
        onEnd?.Invoke();
    }

    private IEnumerator SpawnFlashes()
    {
        float timeBetweenFlashes = RandomHelper.RandomFloat(currentLightningFlashData.MinMaxTimeBetweenFlashes);
        yield return new WaitForSeconds(timeBetweenFlashes);
        SpawnFlash();
        spawnFlashesCoroutine = StartCoroutine(SpawnFlashes());
    }

    private void SpawnFlash()
    {
        Vector3 spawnPos = new Vector3(
            RandomHelper.RandomFloat(currentLightningFlashData.MinMaxAllowedSpawningPosX),
            RandomHelper.RandomFloat(currentLightningFlashData.MinMaxAllowedSpawningPosY),
            RandomHelper.RandomFloat(currentLightningFlashData.MinMaxAllowedSpawningPosZ));
        FlashOfLight light = Instantiate(flashOfLightPrefab, spawnPos, Quaternion.identity);
        light.Activate(0);

        if (currentSpookerFlashData.Enable &&
            RandomHelper.EvaluateChanceTo(currentSpookerFlashData.ChanceToShowSpookerWithFlash)
            && DayNightManager._Instance.CurrentTimeOfDayLabel >= currentSpookerFlashData.BeginAt
            && !GameManager._Instance.InMonsterSequence)
        {
            StartCoroutine(Utils.CallActionAfterDelay(delegate
            {
                Vector3 placePos = player.position + player.forward.normalized * RandomHelper.RandomFloat(currentSpookerFlashData.MinMaxDistanceFromPlayer);
                placePos.y = player.position.y;

                placePos += new Vector3(
                    RandomHelper.RandomFloat(currentSpookerFlashData.MinMaxAllowedSpawningPosX),
                    0,
                    RandomHelper.RandomFloat(currentSpookerFlashData.MinMaxAllowedSpawningPosZ));

                shownByFlashSpooker.position = placePos;
                shownByFlashSpooker.rotation = Quaternion.LookRotation(placePos - player.position);
                jumpscareSpookerBindToGround.TryBind();
                FadeSpooker();
            }, RandomHelper.RandomFloat(currentSpookerFlashData.ShownByFlashSpookerDelay)));
        }
    }

    public void SetWeather(WeatherType type)
    {
        CurrentWeatherType = type;
        WeatherTypeData data = weatherTypeData[type];

        // Stop previous coroutine
        if (spawnFlashesCoroutine != null)
        {
            StopCoroutine(spawnFlashesCoroutine);
            spawnFlashesCoroutine = null;
        }

        // Start new
        if (currentLightningFlashData.Enable)
        {
            spawnFlashesCoroutine = StartCoroutine(SpawnFlashes());
        }

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
