using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class NightLight : MonoBehaviour
{
    private Light light;
    private void Awake()
    {
        light = GetComponent<Light>();
    }

    public void ChangeState(GameObjectState state)
    {
        light.enabled = (state == GameObjectState.ACTIVE ? true : false);
    }
}
