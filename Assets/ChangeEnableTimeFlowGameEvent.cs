using UnityEngine;

public class ChangeEnableTimeFlowGameEvent : GameEvent
{
    [SerializeField] private bool newValue;
    protected override void Activate()
    {
        DayNightManager._Instance.EnableTimeFlow = newValue;
    }
}
