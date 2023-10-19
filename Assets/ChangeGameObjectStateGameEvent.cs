using System.Collections;
using UnityEngine;

public enum GameObjectState
{
    ACTIVE,
    INACTIVE
}

public class ChangeGameObjectStateGameEvent : GameEvent
{
    [SerializeField] private SerializableDictionary<GameObjectState, GameObject[]> gameObjectNewStateMap;

    protected override void Activate()
    {
        if (gameObjectNewStateMap.ContainsKey(GameObjectState.ACTIVE))
        {
            foreach (GameObject obj in gameObjectNewStateMap[GameObjectState.ACTIVE])
            {
                obj.SetActive(true);
            }
        }

        if (gameObjectNewStateMap.ContainsKey(GameObjectState.INACTIVE))
        {
            foreach (GameObject obj in gameObjectNewStateMap[GameObjectState.INACTIVE])
            {
                obj.SetActive(false);
            }
        }
    }
}
