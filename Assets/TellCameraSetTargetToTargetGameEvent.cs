using UnityEngine;

public class TellCameraSetTargetToTargetGameEvent : GameEvent
{
    [SerializeField] private CameraFollowSubject camera;

    [SerializeField] private bool setPos;
    [SerializeField] private bool setRot;
    protected override void Activate()
    {
        if (setPos)
            camera.SetToTargetPos();
        if (setRot)
            camera.SetToTargetRot();
    }
}
