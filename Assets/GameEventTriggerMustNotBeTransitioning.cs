using UnityEngine;

public class GameEventTriggerMustNotBeTransitioning : GameEventTriggerAdditionalCondition
{
    public override bool Condition()
    {
        return !TransitionManager._Instance.Transitioning;
    }
}
