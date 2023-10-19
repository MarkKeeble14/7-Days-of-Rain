using UnityEngine;

public class ChangeTransitionSettingsGameEvent : GameEvent
{
    [SerializeField] private bool useTransition;
    [SerializeField] private float posChangeRate;
    [SerializeField] private float rotChangeRate;

    protected override void Activate()
    {
        GameManager._Instance.ActiveCamera.SetTransitionSettings(useTransition, posChangeRate, rotChangeRate);
    }
}
