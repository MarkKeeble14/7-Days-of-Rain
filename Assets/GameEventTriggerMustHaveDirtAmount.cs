using UnityEngine;

public class GameEventTriggerMustHaveDirtAmount : GameEventTriggerAdditionalCondition
{
    [SerializeField] private float requiredAmount;

    public override bool Condition()
    {
        return GameManager._Instance.CurrentDirtAmount > requiredAmount;
    }
}
