using UnityEngine;

public class GameEventTriggerToDoListItemMustBeOfCheckState : GameEventTriggerAdditionalCondition
{
    [SerializeField] private GameObjectState desiredState;
    [SerializeField] private string key;
    [SerializeField] private ToDoList toDoList;

    public override bool Condition()
    {
        return desiredState == GameObjectState.ACTIVE ? toDoList.IsItemChecked(key) : !toDoList.IsItemChecked(key);
    }
}