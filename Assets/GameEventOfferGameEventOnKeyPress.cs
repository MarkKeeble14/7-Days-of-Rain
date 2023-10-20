using UnityEngine;

public class GameEventOfferGameEventOnKeyPress : GameEvent
{
    [SerializeField] private GameEventTriggerOnKeyPress gameEvent;

    protected override void Activate()
    {
        if (!gameEvent.DoesPassAdditionalShowConditions()) return;
        ShowGameEventTriggerOpporotunity._Instance.TryAddTrigger(gameEvent);
    }
}
