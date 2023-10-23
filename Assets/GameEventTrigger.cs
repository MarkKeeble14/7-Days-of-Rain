using UnityEngine;

[RequireComponent(typeof(GameEvent))]
public abstract class GameEventTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent[] triggersEvents;
    [SerializeField] private GameEventTriggerAdditionalCondition[] additionalShowConditions;
    [SerializeField] private GameEventTriggerAdditionalCondition[] additionalTriggerConditions;
    [SerializeField] private bool destroyOnTrigger;
    private bool canTrigger = true;

    private bool PassesConditions(GameEventTriggerAdditionalCondition[] conditions, bool allowDialogueOnFail)
    {
        if (conditions.Length <= 0) return true;
        foreach (GameEventTriggerAdditionalCondition condition in conditions)
        {
            if (!condition.Condition())
            {
                if (allowDialogueOnFail)
                {
                    DialogueManager._Instance.StartCoroutine(DialogueManager._Instance.ExecuteDialogue(condition.DialogueOnFailCondition));
                }
                return false;
            }
        }
        return true;
    }

    public bool DoesPassAdditionalShowConditions()
    {
        return PassesConditions(additionalShowConditions, false);
    }

    public bool DoesPassAdditionalTriggerConditions()
    {
        return PassesConditions(additionalTriggerConditions, true);
    }

    [ContextMenu("Trigger")]
    private void Trigger()
    {
        Trigger(true);
    }

    public void Trigger(bool overrideChecks = false)
    {
        if (!overrideChecks)
        {
            if (!canTrigger || !DoesPassAdditionalTriggerConditions()) return;
        }
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
