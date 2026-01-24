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
    [Header("Stats")]
    [field:SerializeField]
    private InputActionReference ToggleLookForwardBind;
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
    internal bool LookForward = false;
    internal bool CanMove {get;private set;} = true;
    private bool IsAlive = true;
    private Vector3 CurrentSpeed = new();
    private Rigidbody rb;
    void Start()
    {
        playerHealth.OnDamaged += OnPlayerDamaged;
        rb = GetComponent<Rigidbody>();
        listener.ConnectEventToKeybind(ToggleLookForwardBind,ToggleLookForward);
    }
    void Update()
    {
        if (CanMove)
        {
            HandleMovement();
            HandleLook();
            HandleAnimations();
        }
        cameraBehaviour.CameraLocked = listener.MouseLocked;
    }
    void OnPlayerDamaged(double currentHealth)
    {
        IsAlive = currentHealth>0;
        CanMove = CanMove && IsAlive;
    }
    void HandleMovement()
    {
        
        CurrentSpeed = rb.linearVelocity;

        Vector3 targetSpeed = listener.MovementVector3 * PlayerMaxSpeed;

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
}
