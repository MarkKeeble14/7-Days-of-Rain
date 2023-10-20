using UnityEngine;

public abstract class GameEventTriggerAdditionalCondition : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogueOnFailCondition;
    public Dialogue[] DialogueOnFailCondition => dialogueOnFailCondition;
    public abstract bool Condition();
}
