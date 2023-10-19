using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ContinuousAudioSource : MonoBehaviour
{
    [SerializeField] private bool muteOnAwake;
    public bool Muted { get; set; }

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Muted = muteOnAwake;
    }

    private void Update()
    {
        audioSource.mute = Muted;
    }
}
