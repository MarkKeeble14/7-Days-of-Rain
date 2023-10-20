using UnityEngine;

public class GameEventTriggerOnKeyPress : GameEventTrigger
{
    [SerializeField] private string activationEffectString;
    public string ActivationText => activationEffectString;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool shouldDisableOpporotunityOnTrigger = true;
    public bool ShouldDisableOpperotunityOnTrigger => shouldDisableOpporotunityOnTrigger;

    private void OnDisable()
    {
        if (Utils.ApplicationIsAboutToExitPlayMode()) return;
        ShowGameEventTriggerOpporotunity._Instance.TryRemoveTrigger(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.transform.gameObject, playerLayer)) return;

        ShowGameEventTriggerOpporotunity._Instance.TryRemoveTrigger(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.transform.gameObject, playerLayer)) return;

        if (!DoesPassAdditionalShowConditions()) return;

        ShowGameEventTriggerOpporotunity._Instance.TryAddTrigger(this);
    }
}
