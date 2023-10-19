using UnityEngine;
public class TransitionIntoGameEventGameEvent : GameEvent
{
    [SerializeField] private GameEvent[] gameEvents;
    [SerializeField] private string animationName;
    [SerializeField] private float delayAfterOut;
    [SerializeField] private float delayAfterIn;
    [SerializeField] private bool lockInput;

    protected override void Activate()
    {
        AnimationActionSequenceEntry[] animationSequence = new AnimationActionSequenceEntry[2];
        animationSequence[0] = new AnimationActionSequenceEntry(animationName, null, () => ActivateGameEvents(), delayAfterOut, false, lockInput);
        animationSequence[1] = new AnimationActionSequenceEntry(animationName, null, null, delayAfterIn, true, lockInput);

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