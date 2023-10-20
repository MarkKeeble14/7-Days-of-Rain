using UnityEngine;

public class GameEventTriggerMustNotHaveCrossedOutToday : GameEventTriggerAdditionalCondition
{
    public override bool Condition()
    {
        return !DayNightManager._Instance.HasCrossedOutToday;
    }
}