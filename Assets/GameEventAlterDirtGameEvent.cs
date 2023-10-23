using UnityEngine;

public class GameEventAlterDirtGameEvent : GameEvent
{
    [SerializeField] private float alterBy;
    protected override void Activate()
    {
        GameManager._Instance.AlterDirt(alterBy);
    }
}
