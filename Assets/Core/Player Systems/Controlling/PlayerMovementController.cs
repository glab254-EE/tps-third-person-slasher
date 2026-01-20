using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [field:SerializeField]
    private AnimatorHandler animator;
    [field:SerializeField]
    private PlayerInputListener listener;
    [field:SerializeField]

    private CameraBehaviour cameraBehaviour;
    [field:SerializeField]
    private PlayerHealthHandler playerHealth;
    [field:SerializeField]
    private float MoveAnimationThreshold = 0.1f;
    [field:SerializeField]
    private float PlayerMaxSpeed;
    [field:SerializeField]
    private float PlayerAcceloration = 2;
    [field:SerializeField]
    internal bool LookForward = false;
    [field:SerializeField]
    private float PlayerTurnSpeed = 10;
    internal bool CanMove {get;private set;} = true;
    private bool IsAlive = true;
    private Vector3 CurrentSpeed = new();
    private Rigidbody rb;
    void Start()
    {
        playerHealth.OnDamaged += OnPlayerDamaged;
        rb = GetComponent<Rigidbody>();
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
        Quaternion targetLookVector = new();

        Vector3 NoYVector = rb.linearVelocity;

        if (LookForward)
        {
            NoYVector = listener.CameraTransform.forward;
        }

        NoYVector.y = 0;

        NoYVector.Normalize();

        if (NoYVector == Vector3.zero)
            return;

        targetLookVector = Quaternion.LookRotation(NoYVector);

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
}
