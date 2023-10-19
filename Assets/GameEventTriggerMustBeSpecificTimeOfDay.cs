using System.Collections.Generic;
using UnityEngine;

public class GameEventTriggerMustBeSpecificTimeOfDay : GameEventTriggerAdditionalCondition
{
    [SerializeField] private List<TimeOfDayLabel> acceptedTimesOfDay = new List<TimeOfDayLabel>();

    public override bool Condition()
    {
        return acceptedTimesOfDay.Contains(DayNightManager._Instance.CurrentTimeOfDayLabel);
    }
}
