using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestCropGameEvent : GameEvent
{
    [SerializeField] private Crop crop;

    protected override void Activate()
    {
        crop.Harvest();
    }
}
