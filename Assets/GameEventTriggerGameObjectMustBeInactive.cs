using UnityEngine;

public class GameEventTriggerGameObjectMustBeInactive : GameEventTriggerAdditionalCondition
{
    [SerializeField] private GameObject obj;
    [SerializeField] private GameObjectState requiredState;

    public override bool Condition()
    {
        return requiredState == GameObjectState.ACTIVE ? obj.activeInHierarchy : !obj.activeInHierarchy;
    }
}