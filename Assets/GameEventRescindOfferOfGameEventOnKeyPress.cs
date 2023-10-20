using UnityEngine;

public class GameEventRescindOfferOfGameEventOnKeyPress : GameEvent
{
    [SerializeField] private GameEventTriggerOnKeyPress gameEvent;

    protected override void Activate()
    {
        ShowGameEventTriggerOpporotunity._Instance.TryRemoveTrigger(gameEvent);
    }
}
