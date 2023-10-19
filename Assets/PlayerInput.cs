using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool LockInput { get; set; }
    public bool MoveUp => Input.GetKey(KeyCode.W) && !LockInput;
    public bool MoveLeft => Input.GetKey(KeyCode.A) && !LockInput;
    public bool MoveDown => Input.GetKey(KeyCode.S) && !LockInput;
    public bool MoveRight => Input.GetKey(KeyCode.D) && !LockInput;
    public bool Sprinting => Input.GetKey(KeyCode.LeftShift)
        && (GameManager._Instance.CurrentLocationState == PlayerLocationState.EXTERIOR || GameManager._Instance.CurrentLocationState == PlayerLocationState.SPAWN) && !LockInput;
    public bool LookUp => Input.GetKey(KeyCode.UpArrow) && !LockInput;
    public bool LookDown => Input.GetKey(KeyCode.DownArrow) && !LockInput;
    public bool TurnRight => Input.GetKey(KeyCode.RightArrow) && !LockInput;
    public bool TurnLeft => Input.GetKey(KeyCode.LeftArrow) && !LockInput;

}
