using UnityEngine;

public class GrowCropGameEvent : GameEvent
{
    [SerializeField] private Crop crop;

    protected override void Activate()
    {
        crop.Grow();
    }
}