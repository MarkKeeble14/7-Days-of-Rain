using UnityEngine;

public class GameEventTriggerCropMustBeReadyForHarvestState : GameEventTriggerAdditionalCondition
{
    [SerializeField] private Crop crop;
    [SerializeField] private bool invert;
    public override bool Condition()
    {
        return !invert ? crop.ReadyForHarvest : !crop.ReadyForHarvest;
    }
}
