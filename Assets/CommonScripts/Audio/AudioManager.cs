using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private SerializableDictionary<string, SimpleAudioClipContainer> sfxDict = new SerializableDictionary<string, SimpleAudioClipContainer>();

    [Header("References")]
    [SerializeField] private AudioMixer mixer;
    private List<AudioSource> audioSourceArray;
    [SerializeField] private Transform audioSourceHolder;
    [SerializeField] private Transform parentSpawnedTo;
    [SerializeField] private AudioSource audioSourcePrefab;

    [Header("Settings")]
    [SerializeField] private float minPercent = 0.0001f;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private float defaultMusicVolume = 0.8f;

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private float defaultSFXVolume = 0.8f;

    [SerializeField] private Slider ambienceVolumeSlider;
    [SerializeField] private float defaultAmbienceVolume = 0.8f;

    public static AudioManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _Instance = this;

        audioSourceArray = new List<AudioSource>();
        audioSourceArray.AddRange(audioSourceHolder.GetComponentsInChildren<AudioSource>());
    }

    private void Start()
    {
        // SFX Volume
        SetSFXVolume(defaultSFXVolume);
        sfxVolumeSlider.value = defaultSFXVolume;

        // Ambience Volume
        SetAmbienceVolume(defaultAmbienceVolume);
        ambienceVolumeSlider.value = defaultAmbienceVolume;
    }

    private AudioSource GetAudioSource()
    {
        if (audioSourceArray.Count == 0)
        {
            AudioSource spawned = Instantiate(audioSourcePrefab, parentSpawnedTo);
            audioSourceArray.Add(spawned);
        }
        return audioSourceArray[0];
    }

    private IEnumerator PlayFromSourceUninterrupted(SimpleAudioClipContainer clip)
    {
        AudioSource source = GetAudioSource();

        audioSourceArray.Remove(source);
        clip.Source = source;
        source.volume = clip.Volume;

        clip.Play();

        yield return new WaitUntil(() => !source.isPlaying);

        audioSourceArray.Add(source);
    }

    public void PlayFromSFXDict(string key)
    {
        StartCoroutine(PlayFromSourceUninterrupted(sfxDict[key]));
    }

    public void SetSFXVolume(float percent)
    {
        if (percent <= 0) percent = minPercent;
        mixer.SetFloat("SFXVol", Mathf.Log10(percent) * 20);
    }

    public void SetAmbienceVolume(float percent)
    {
        if (percent <= 0) percent = minPercent;
        mixer.SetFloat("AmbienceVol", Mathf.Log10(percent) * 20);
    }
}
