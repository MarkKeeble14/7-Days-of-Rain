using UnityEngine;

public class PlayAudioGameEvent : GameEvent
{
    [SerializeField] private SimpleAudioClipContainer clip;

    protected override void Activate()
    {
        clip.PlayOneShot();
    }
}
