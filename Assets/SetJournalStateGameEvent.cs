using UnityEngine;

public class SetJournalStateGameEvent : GameEvent
{
    [SerializeField] private bool newJournalState;

    [SerializeField] private Journal journal;

    protected override void Activate()
    {
        journal.IsJournalActive = newJournalState;
    }
}
