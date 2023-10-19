using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GameEventTriggerOnLook : GameEventTrigger
{
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            if (hit.transform == transform)
            {
                Trigger();
            }
        }
    }
}