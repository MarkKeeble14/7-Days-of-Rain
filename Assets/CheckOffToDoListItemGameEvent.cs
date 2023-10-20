using UnityEngine;

public class CheckOffToDoListItemGameEvent : GameEvent
{
    [SerializeField] private string key;
    protected override void Activate()
    {
        GameManager._Instance.CheckToDoItem(key);
    }
}
