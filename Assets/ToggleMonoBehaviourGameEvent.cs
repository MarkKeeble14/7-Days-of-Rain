using UnityEngine;

public class ToggleMonoBehaviourGameEvent : GameEvent
{
    [SerializeField] private MonoBehaviour script;
    protected override void Activate()
    {
        script.enabled = !script.enabled;
    }
}
