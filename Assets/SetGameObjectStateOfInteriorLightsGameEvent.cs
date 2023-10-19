using UnityEngine;

public class SetGameObjectStateOfInteriorLightsGameEvent : GameEvent
{
    [SerializeField] private GameObjectState state;

    protected override void Activate()
    {
        GameManager._Instance.StartCoroutine(GameManager._Instance.SetStateOfInteriorLights(state));
    }
}