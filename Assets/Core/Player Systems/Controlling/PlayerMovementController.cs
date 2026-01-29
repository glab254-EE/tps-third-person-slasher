using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Set-Up")]
    [field:SerializeField]
    private AnimatorHandler animator;
    [field:SerializeField]
    private PlayerInputListener listener;
    [field:SerializeField]
    private CameraBehaviour cameraBehaviour;
    [field:SerializeField]
    private PlayerHealthHandler playerHealth;
    [Header("Input")]
    [field:SerializeField]
    private InputActionReference ToggleLookForwardBind;
    [field:SerializeField]
    private InputActionReference RollKey;
    [field:SerializeField]
    private bool DefaultEnabledState = true;
    [Header("Animations")]
    [field:SerializeField]
    private float MoveAnimationThreshold = 0.1f;
    [Header("Stats")]
    [field:SerializeField]
    private float PlayerMaxSpeed;
    [field:SerializeField]
    private float PlayerAcceloration = 2;
    [field:SerializeField]
    private float PlayerTurnSpeed = 10;
    [field:SerializeField]
    private float PlayerRollSpeed = 4;
    [field:SerializeField]
    private float PlayerRollDuration = 1f;
    [field:SerializeField]
    private float PlayerRollForceDuration = 1f;
    [field:SerializeField]
    private float PlayerRollCooldown = 1f;
    internal bool LookForward = false;
    internal bool CanMove {get;private set;} = true;
    internal bool CanRoll {get;private set;} = true;
    private bool IsAlive = true;
    private Vector3 CurrentSpeed = new();
    private Vector3 OverrideTargetSpeed = new();
    private Rigidbody rb;
    void Start()
    {
        listener.MouseLocked = DefaultEnabledState;
        playerHealth.OnDamaged += OnPlayerDamaged;
        rb = GetComponent<Rigidbody>();
        listener.ConnectEventToKeybind(ToggleLookForwardBind,ToggleLookForward);
        listener.ConnectEventToKeybind(RollKey,OnRollButtonPress);
    }
    void Update()
    {
        if (CanMove)
        {
            HandleMovement();
            HandleLook();
        }
        HandleAnimations();
        cameraBehaviour.CameraLocked = listener.MouseLocked;
    }
    void OnPlayerDamaged(double currentHealth)
    {
        IsAlive = currentHealth>0;
        CanMove = CanMove && IsAlive;
        CanRoll = IsAlive;
    }
    void HandleMovement()
    {
        CurrentSpeed = rb.linearVelocity;

        Vector3 targetSpeed = listener.MovementVector3 * PlayerMaxSpeed;

        if (OverrideTargetSpeed != Vector3.zero) targetSpeed = OverrideTargetSpeed;

        targetSpeed.y = CurrentSpeed.y;

        Vector3 slerpedSpeed = Vector3.Slerp(CurrentSpeed,targetSpeed,PlayerAcceloration*Time.deltaTime);

        rb.linearVelocity = slerpedSpeed;
    }
    void HandleLook()
    {
        Vector3 NoYVector = rb.linearVelocity;

        if (LookForward)
        {
            NoYVector = listener.CameraTransform.forward;
        }

        NoYVector.y = 0;

        NoYVector.Normalize();

        if (NoYVector == Vector3.zero)
            return;

        Quaternion targetLookVector = Quaternion.LookRotation(NoYVector);

        transform.rotation = Quaternion.Slerp(transform.rotation,targetLookVector,PlayerTurnSpeed*Time.deltaTime);
    }
    void HandleAnimations()
    {
        if (rb.linearVelocity.magnitude > MoveAnimationThreshold)
        {
            animator.SetAnimatorBool("Walking",true);
        }
        else
        {
            animator.SetAnimatorBool("Walking",false);
        }
    }
    void ToggleLookForward(InputAction.CallbackContext _)
    {
        LookForward = !LookForward;
    }
    void OnRollButtonPress(InputAction.CallbackContext callbackContext)
    {
        if (CanMove && CanRoll && callbackContext.ReadValueAsButton() && listener.MovementVector3.magnitude > 0)
        {
            animator.SetAnimatorTrigger("Roll");
            Task.Run(RollTask);
        }
    }
    Task RollTask()
    {

        bool _oldlook = LookForward;


        Vector3 _targetSpeed = listener.MovementVector3.normalized * PlayerRollSpeed;

        CanRoll = false;
        
        LookForward = false;

        OverrideTargetSpeed = _targetSpeed;

        Task.Delay(Mathf.RoundToInt(PlayerRollForceDuration*1000)).Wait();

        OverrideTargetSpeed = Vector3.zero;

        Task.Delay(Mathf.RoundToInt(PlayerRollDuration*1000)).Wait();

        LookForward = _oldlook;

        Task.Delay(Mathf.RoundToInt(PlayerRollCooldown*1000)).Wait();

        CanRoll = true;

        return Task.CompletedTask;
    }
}
