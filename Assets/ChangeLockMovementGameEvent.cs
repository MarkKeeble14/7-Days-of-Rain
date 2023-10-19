using UnityEngine;

public class ChangeLockMovementGameEvent : GameEvent
{
    [SerializeField] private bool newValue;

    protected override void Activate()
    {
        GameManager._Instance.LockMovement = newValue;
    }
}
