using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputListener : MonoBehaviour
{
    [field:SerializeField]
    internal Transform CameraTransform {get; private set;}
    [field:SerializeField]
    internal Vector2 AxisOutput {get;private set;}= new();
    [field:SerializeField]
    internal Vector3 MovementVector3 {get;private set;}= new();
    [field:SerializeField]
    internal bool MouseLocked {get;private set;} = false;
    private InputSystem_Actions inputActions;
    void Start()
    {
     inputActions??=new(); 
     inputActions.Player.CameraLock.performed += OnMouseLock;
     inputActions.Player.Enable(); 
    }
    void OnDestroy()
    {
     inputActions.Player.Disable();         
    }
    void Update()
    {
        AxisOutput = inputActions.Player.Move.ReadValue<Vector2>();
        
        Vector3 _movementVector = CameraTransform.forward*AxisOutput.y+CameraTransform.right*AxisOutput.x;
        _movementVector.y = 0;
        MovementVector3 = _movementVector.normalized;
    }
    void OnMouseLock(InputAction.CallbackContext t)
    {
        if (!t.ReadValueAsButton()) return;
        MouseLocked = !MouseLocked;
    }
}
