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
}
