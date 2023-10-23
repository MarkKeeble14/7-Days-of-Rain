using System.Collections.Generic;
using UnityEngine;

public class TransitionIntoGameEventGameEvent : GameEvent
{
    [SerializeField] private GameEvent[] preOutGameEvents;
    [SerializeField] private GameEvent[] postOutGameEvents;
    [SerializeField] private GameEvent[] preInGameEvents;
    [SerializeField] private GameEvent[] postInGameEvents;
    [SerializeField] private string animationName;
    [SerializeField] private float delayAfterOut;
    [SerializeField] private float delayAfterIn;
    [SerializeField] private bool lockInput;
    [SerializeField] private bool useIn = true;
    [SerializeField] private bool useOut = true;

    protected override void Activate()
    {
        List<AnimationActionSequenceEntry> animationSequence = new List<AnimationActionSequenceEntry>();
        if (useOut)
        {
            animationSequence.Add(new AnimationActionSequenceEntry(animationName,
                () => ActivateGameEvents(preOutGameEvents),
                () => ActivateGameEvents(postOutGameEvents),
                delayAfterOut, false, lockInput));
        }
        if (useIn)
        {
            animationSequence.Add(new AnimationActionSequenceEntry(animationName,
                () => ActivateGameEvents(preInGameEvents),
                () => ActivateGameEvents(postInGameEvents),
                delayAfterIn, true, lockInput));
        }

        TransitionManager._Instance.StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }

    private void ActivateGameEvents(GameEvent[] events)
    {
        foreach (GameEvent gameEvent in events)
        {
            gameEvent.CallActivate();
        }
    }
}