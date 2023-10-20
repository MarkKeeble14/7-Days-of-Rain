using UnityEngine;

public class ChangeCanSprintGameEvent : GameEvent
{
    [SerializeField] private bool newValue;
    protected override void Activate()
    {
        CanSprintDisplayController._Instance.CanSprint = newValue;
    }
}
