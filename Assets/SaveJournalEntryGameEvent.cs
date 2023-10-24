public class SaveJournalEntryGameEvent : GameEvent
{
    protected override void Activate()
    {
        GameManager._Instance.SaveJournalEntry();
    }
}
