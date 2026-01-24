using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputListener : MonoBehaviour
{
    [field:Header("Set-Up")]
    [field:SerializeField]
    private InputActionReference inputActionForCameraLock;
    [field:SerializeField]
    internal Transform CameraTransform {get; private set;}
    internal Vector2 AxisOutput {get;private set;}= new();
    internal Vector3 MovementVector3 {get;private set;}= new();
    internal bool MouseLocked {get;private set;} = false;
    private Dictionary<InputActionReference,UnityEvent<InputAction.CallbackContext>> Connections = new();
    private InputSystem_Actions inputActions;
    void Start()
    {

     inputActions??=new(); 

     ConnectEventToKeybind(inputActionForCameraLock,OnMouseLock);
     inputActions.Player.Enable(); 
    }
    void OnDestroy()
    {
        inputActions.Player.Disable();         
    }
    void Update()
    {
        AxisOutput = inputActions.Player.Move.ReadValue<Vector2>();

        if (CameraTransform == null)
        {
            CameraTransform = Camera.main.transform;
        }

        Vector3 _movementVector = CameraTransform.forward*AxisOutput.y+CameraTransform.right*AxisOutput.x;

        _movementVector.y = 0;

        MovementVector3 = _movementVector.normalized;
    }
    public void ConnectEventToKeybind(InputActionReference keybind, UnityAction<InputAction.CallbackContext> action)
    {
        if (!Connections.ContainsKey(keybind))
        {
            Connections.Add(keybind,new());

            keybind.action.performed += callback => Connections[keybind]?.Invoke(callback);

            if (!keybind.action.enabled) 
                keybind.action.Enable();
        }
        Connections[keybind].AddListener(action);
    }
    public void DissconectEventFromKeybind(InputActionReference keybind, UnityAction<InputAction.CallbackContext> action)
    {
        if (Connections.ContainsKey(keybind))
        {
            Connections[keybind]?.RemoveListener(action);
        }
    }
    private void OnMouseLock(InputAction.CallbackContext t)
    {
        if (!t.ReadValueAsButton()) return;
        MouseLocked = !MouseLocked;
    }
}
