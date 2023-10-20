using System.Collections.Generic;
using UnityEngine;

public class TransitionIntoGameEventGameEvent : GameEvent
{
    [SerializeField] private GameEvent[] gameEvents;
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
            animationSequence.Add(new AnimationActionSequenceEntry(animationName, null, () => ActivateGameEvents(), delayAfterOut, false, lockInput));
        if (useIn)
            animationSequence.Add(new AnimationActionSequenceEntry(animationName, null, null, delayAfterIn, true, lockInput));

        TransitionManager._Instance.StartCoroutine(TransitionManager._Instance.PlayAnimationWithActionsInBetween(animationSequence));
    }

    private void ActivateGameEvents()
    {
        foreach (GameEvent gameEvent in gameEvents)
        {
            gameEvent.CallActivate();
        }
    }
}