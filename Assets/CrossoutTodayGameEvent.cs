using UnityEngine;

public class CrossoutTodayGameEvent : GameEvent
{
    protected override void Activate()
    {
        DayNightManager._Instance.CrossoutCurrentDay();
    }
}