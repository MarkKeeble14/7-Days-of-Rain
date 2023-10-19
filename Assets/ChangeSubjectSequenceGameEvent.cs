using UnityEngine;

public class ChangeSubjectSequenceGameEvent : GameEvent
{
    [SerializeField] private CameraFollowSubjectData[] data;
    [SerializeField] private float delay;

    protected override void Activate()
    {
        GameManager._Instance.StartCoroutine(GameManager._Instance.ActiveCamera.ShowNewSubjectSequence(data, delay));
    }
}
