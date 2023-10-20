using UnityEngine;

public class GameEventTriggerCropMustNotBeDead : GameEventTriggerAdditionalCondition
{
    [SerializeField] private Crop crop;
    public override bool Condition()
    {
        return !crop.IsDead;
    }
}
