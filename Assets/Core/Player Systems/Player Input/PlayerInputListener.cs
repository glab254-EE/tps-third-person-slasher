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
    internal bool MouseLocked = false;
    private Dictionary<InputActionReference,Dictionary<string,UnityAction<InputAction.CallbackContext,bool>>> Connections = new();
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
    public void ConnectEventToKeybind(InputActionReference keybind, UnityAction<InputAction.CallbackContext> action,bool activateOnCancel = false,bool once = false)
    {
        if (!Connections.ContainsKey(keybind))
        {
            Connections.Add(keybind,new());

            keybind.action.performed += callback => SetupConnectionCallbacks(Connections[keybind],callback,true);
            keybind.action.canceled += callback => SetupConnectionCallbacks(Connections[keybind],callback,false);

            if (!keybind.action.enabled) 
                keybind.action.Enable();
        }
        Connections[keybind].Add(action.ToString(),(callbackContext, active) =>
        {
            if (activateOnCancel || active)
            {
                action(callbackContext);    
                if (once)
                {
                    DissconectEventFromKeybind(keybind,action);
                }            
            }
        });
    }
    public void DissconectEventFromKeybind(InputActionReference keybind, UnityAction<InputAction.CallbackContext> action)
    {
        if (Connections.ContainsKey(keybind))
        {
            Connections[keybind]?.Remove(action.ToString());
        }
    }
    private void SetupConnectionCallbacks(Dictionary<string,UnityAction<InputAction.CallbackContext,bool>> connected,InputAction.CallbackContext callback,bool active = true)
    {
        if (connected == null) return;
        foreach (KeyValuePair<string,UnityAction<InputAction.CallbackContext,bool>> pairs in connected)
        {
            pairs.Value?.Invoke(callback,active);
        }
    }
    private void OnMouseLock(InputAction.CallbackContext callBack)
    {
        if (!callBack.ReadValueAsButton()) return;
        MouseLocked = !MouseLocked;
    }
}
