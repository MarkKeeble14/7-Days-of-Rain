using UnityEngine;

public class GameEventTriggerOnEnter : GameEventTrigger
{
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.transform.gameObject, playerLayer)) return;
        Trigger();
    }
}
