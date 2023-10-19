using UnityEngine;

public class GameEventTriggerMustNotHaveGrownToday : GameEventTriggerAdditionalCondition
{
    [SerializeField] private Crop crop;
    public override bool Condition()
    {
        return !crop.HasGrownToday;
    }
}
