public class EndChaseSequencesGameEvent : GameEvent
{
    protected override void Activate()
    {
        GameManager._Instance.EndMonsterSequence();
        GameManager._Instance.EndCoyoteSequence();
    }
}
