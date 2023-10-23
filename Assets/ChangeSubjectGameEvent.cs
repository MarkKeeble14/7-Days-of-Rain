using UnityEngine;

public class ChangeSubjectGameEvent : GameEvent
{
    [SerializeField] private CameraFollowSubjectData data;

    protected override void Activate()
    {
        GameManager._Instance.CameraController.SetNewSubject(data);
    }
}