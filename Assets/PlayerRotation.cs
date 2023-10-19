using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private Quaternion targetRotation;
    [SerializeField] private PlayerInput p_Input;

    private void Update()
    {
        if (GameManager._Instance.LockMovement) return;

        Quaternion rotationAmount = Quaternion.identity;
        if (p_Input.TurnLeft)
        {
            rotationAmount = Quaternion.Euler(0, -90, 0);
        }
        if (p_Input.TurnRight)
        {
            rotationAmount = Quaternion.Euler(0, 90, 0);
        }

        targetRotation = transform.rotation * rotationAmount;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
