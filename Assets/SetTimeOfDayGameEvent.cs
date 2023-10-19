using UnityEngine;

public enum TimeOfDayLabel
{
    MORNING,
    MIDDAY,
    EVENING,
    NIGHT,
    MIDNIGHT
}

public class SetTimeOfDayGameEvent : GameEvent
{
    [SerializeField] private TimeOfDayLabel todLabel;

    protected override void Activate()
    {
        DayNightManager._Instance.SetTimeOfDay(todLabel);
    }
}
