public class GameEventTriggerMustNotBeBeingForcedToSleep : GameEventTriggerAdditionalCondition
{
    public override bool Condition()
    {
        return !GameManager._Instance.BeingForcedToSleep;
    }
}
