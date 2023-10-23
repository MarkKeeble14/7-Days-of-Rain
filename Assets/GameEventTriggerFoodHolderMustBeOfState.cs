using UnityEngine;

public class GameEventTriggerFoodHolderMustBeOfState : GameEventTriggerAdditionalCondition
{
    [SerializeField] private GameObjectState desiredState;
    [SerializeField] private AnimalFoodHolder foodHolder;
    public override bool Condition()
    {
        return desiredState == GameObjectState.ACTIVE ? foodHolder.IsFilled : !foodHolder.IsFilled;
    }
}
