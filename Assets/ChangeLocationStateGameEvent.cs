using System.Collections;
using UnityEngine;

public class ChangeLocationStateGameEvent : GameEvent
{
    [SerializeField] private PlayerLocationState changeTo;
    [SerializeField] private bool playAudio;

    protected override void Activate()
    {
        GameManager._Instance.LoadLocationState(changeTo, playAudio);
    }
}
