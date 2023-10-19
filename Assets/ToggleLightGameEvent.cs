using UnityEngine;

public class ToggleLightGameEvent : GameEvent
{
    [SerializeField] private Light light;
    protected override void Activate()
    {
        light.enabled = !light.enabled;
    }
}
