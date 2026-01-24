using Unity.Cinemachine;
using UnityEngine;
[RequireComponent(typeof(CinemachineInputAxisController))]
public class CameraBehaviour : MonoBehaviour
{
    [field:SerializeField]
    internal bool CameraControlsEnabled = true;
    [field:SerializeField]
    internal bool CameraLocked = false;
    private CinemachineInputAxisController controller;
    void Start()
    {
        controller = GetComponent<CinemachineInputAxisController>();
    }
    void Update()
    {
        if (CameraControlsEnabled && CameraLocked && !IsInputsEnabled())
        {
            SetEnabledInputs(true);
        } else if ((!CameraControlsEnabled || !CameraLocked) && IsInputsEnabled())
        {
            SetEnabledInputs(false);
        }
        if (CameraLocked && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else if (!CameraLocked && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;            
        }
    }
    private bool IsInputsEnabled()
    {
        bool isenabled = false;
        foreach (var c in controller.Controllers)
        {
            if (c.Name == "Look Orbit X" || c.Name == "Look Orbit Y")
            {
                if (!isenabled && c.Enabled) isenabled = true;
            }
        }
        return isenabled;
    }
    private void SetEnabledInputs(bool enabled)
    {
        
        foreach (var c in controller.Controllers)
        {
            if (c.Name == "Look Orbit X" || c.Name == "Look Orbit Y")
            {
                c.Enabled = enabled;
            }
        }
    }
}
