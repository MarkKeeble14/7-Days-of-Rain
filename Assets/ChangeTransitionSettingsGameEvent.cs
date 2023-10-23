using UnityEngine;

public class ChangeTransitionSettingsGameEvent : GameEvent
{
    [SerializeField] private float posChangeRate;
    [SerializeField] private float rotChangeRate;

    protected override void Activate()
    {
        GameManager._Instance.CameraController.SetTransitionSettings(posChangeRate, rotChangeRate);
    }
}
