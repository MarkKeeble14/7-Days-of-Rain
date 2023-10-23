using UnityEngine;

public class GameEventTriggerMustHaveLastTransitionDirection : GameEventTriggerAdditionalCondition
{
    [SerializeField] private TransitionDirection transitionDirection;
    [SerializeField] private bool desired;
    public override bool Condition()
    {
        bool lastDirectionMatches = TransitionManager._Instance.LastPlayedTransitionDirection == transitionDirection;
        return desired ? lastDirectionMatches : !lastDirectionMatches;
    }
}
