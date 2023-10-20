using UnityEngine;

public class AlterTransformGameEvent : GameEvent
{
    [SerializeField] private Transform transformToAlter;

    [Header("Settings")]
    [SerializeField] private bool alterEuler;
    [SerializeField] private Vector3 newEulerAngles;
    [SerializeField] private bool alterPos;
    [SerializeField] private Vector3 newPosition;

    protected override void Activate()
    {
        if (alterEuler)
        {
            transformToAlter.localEulerAngles = newEulerAngles;
        }
        if (alterPos)
        {
            transformToAlter.position = newPosition;
        }
    }
}
