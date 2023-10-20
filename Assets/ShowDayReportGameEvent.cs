public class ShowDayReportGameEvent : GameEvent
{
    protected override void Activate()
    {
        GameManager._Instance.StartCoroutine(GameManager._Instance.DayReport());
    }
}
