using System.Collections.Generic;
using UnityEngine;

public class GameEventTriggerMustBeOfSpecifiedLocationState : GameEventTriggerAdditionalCondition
{
    [SerializeField] private List<PlayerLocationState> acceptedStates;

    public override bool Condition()
    {
        return acceptedStates.Contains(GameManager._Instance.CurrentLocationState);
    }
}