public class SleptGameEvent : GameEvent
{
    protected override void Activate()
    {
        GameManager._Instance.StartCoroutine(GameManager._Instance.Sleep());
    }
}
