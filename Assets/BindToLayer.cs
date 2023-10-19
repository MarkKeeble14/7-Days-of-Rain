using UnityEngine;

public class BindToLayer : MonoBehaviour
{
    [SerializeField] private LayerMask canBindTo;

    public void TryBind()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector2.down, out hit, Mathf.Infinity))
        {
            if (LayerMaskHelper.IsInLayerMask(hit.transform.gameObject, canBindTo))
            {
                transform.position = hit.transform.position;
                return;
            }
        }
        Destroy(gameObject);
    }
}