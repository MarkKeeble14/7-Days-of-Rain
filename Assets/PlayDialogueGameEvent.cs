using UnityEngine;

public class PlayDialogueGameEvent : GameEvent
{
    [SerializeField] private Dialogue[] dialogue;

    [ContextMenu("Activate")]
    protected override void Activate()
    {
        DialogueManager._Instance.StartCoroutine(DialogueManager._Instance.ExecuteDialogue(dialogue));
    }
}
