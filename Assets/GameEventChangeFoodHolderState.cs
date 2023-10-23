using UnityEngine;

public class GameEventChangeFoodHolderState : GameEvent
{
    [SerializeField] private AnimalFoodHolder foodHolder;
    [SerializeField] private bool newState;
    protected override void Activate()
    {
        foodHolder.IsFilled = newState;
    }
}