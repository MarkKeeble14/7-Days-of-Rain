using UnityEngine;

public class GameEventTriggerOnKeyPress : GameEventTrigger
{
    [SerializeField] private string activationEffectString;

    [SerializeField] private LayerMask playerLayer;

    public string ActivationText => "'" + GameManager._Instance.PlayerInteractKey.ToString() + "' to " + activationEffectString;

    private void OnDisable()
    {
        ShowGameEventTriggerOpporotunity._Instance.RemoveTrigger(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.transform.gameObject, playerLayer)) return;

        ShowGameEventTriggerOpporotunity._Instance.RemoveTrigger(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.transform.gameObject, playerLayer)) return;

        ShowGameEventTriggerOpporotunity._Instance.TryAddTrigger(this);
    }
}
