using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterColdGameEvent : GameEvent
{
    [SerializeField] private float alterBy;
    protected override void Activate()
    {
        GameManager._Instance.AlterCold(alterBy);
    }
}
