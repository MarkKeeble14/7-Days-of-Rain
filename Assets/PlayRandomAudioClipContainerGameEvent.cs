using UnityEngine;

public class PlayRandomAudioClipContainerGameEvent : GameEvent
{
    [SerializeField] private RandomClipAudioClipContainer clip;
    protected override void Activate()
    {
        clip.PlayOneShot();
    }
}