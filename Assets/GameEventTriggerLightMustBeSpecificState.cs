using UnityEngine;

public class GameEventTriggerLightMustBeSpecificState : GameEventTriggerAdditionalCondition
{
    [SerializeField] private Light light;
    [SerializeField] private GameObjectState desiredState;
    public override bool Condition()
    {
        return desiredState == GameObjectState.ACTIVE ? light.enabled : !light.enabled;
    }
}
