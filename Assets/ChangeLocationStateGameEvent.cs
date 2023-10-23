using System.Collections;
using UnityEngine;

public class ChangeLocationStateGameEvent : GameEvent
{
    [SerializeField] private PlayerLocationState changeTo;
    [SerializeField] private PlayAudioGameEvent accompanyingAudio;

    protected override void Activate()
    {
        GameManager._Instance.LoadLocationState(changeTo, accompanyingAudio);
    }
}
