using UnityEngine;

[RequireComponent(typeof(GameEvent))]
public abstract class GameEventTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent[] triggersEvents;
    [SerializeField] private GameEventTriggerAdditionalCondition[] additionalTriggerConditions;
    [SerializeField] private bool destroyOnTrigger;
    private bool canTrigger = true;

    public bool PassesAdditionalConditions
    {
        get
        {
            if (additionalTriggerConditions.Length <= 0) return true;
            foreach (GameEventTriggerAdditionalCondition condition in additionalTriggerConditions)
            {
                if (!condition.Condition())
                    return false;
            }
            return true;
        }
    }


    public void Trigger()
    {
        if (!canTrigger || !PassesAdditionalConditions) return;
        canTrigger = false;

        // Activate all attatched Events
        foreach (GameEvent e in triggersEvents)
        {
            e.CallActivate();
        }

        // Destroy if told to
        if (destroyOnTrigger)
        {
            Destroy(gameObject);
        }

        // Otherwise for now just allow re-trigger
        canTrigger = true;
    }
}
