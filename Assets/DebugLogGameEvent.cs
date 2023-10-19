using UnityEngine;

public class DebugLogGameEvent : GameEvent
{
    [SerializeField] private string s;

    protected override void Activate()
    {
        Debug.Log(s);
    }
}
