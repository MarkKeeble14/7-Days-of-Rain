using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeedModifier;
    [SerializeField] private PlayerInput p_Input;

    // Update is called once per frame
    void Update()
    {
        if (GameManager._Instance.LockMovement) return;

        Vector3 moveDirection = Vector3.zero;
        if (p_Input.MoveRight)
        {
            moveDirection.x += 1;
        }
        if (p_Input.MoveLeft)
        {
            moveDirection.x -= 1;
        }
        if (p_Input.MoveUp)
        {
            moveDirection.z += 1;
        }
        if (p_Input.MoveDown)
        {
            moveDirection.z -= 1;
        }

        Vector3 forwardMovement = transform.forward * moveDirection.z;
        Vector3 horizontalMovement = transform.right * moveDirection.x;
        Vector3 movement = Vector3.ClampMagnitude(forwardMovement + horizontalMovement, 1);
        transform.Translate(movement * moveSpeed * (p_Input.Sprinting ? sprintSpeedModifier : 1) * GameManager._Instance.SatedSpeedModifier * Time.deltaTime, Space.World);
    }
}
