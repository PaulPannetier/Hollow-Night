using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector3 velocity;
    private LayerMask mapsMask;
    private new Transform transform;
    private RaycastHit groundRaycast;
    private float slopeAngle;
    private float currentAngle;
    private bool isGrounded => groundRaycast.collider != null;

#if UNITY_EDITOR
    [SerializeField] private bool drawGizmos;
#endif

    [Header("Detection")]
    [SerializeField] private float groundRaycastLength;

    [Header("Walk")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float walkSpeedLerp;
    [SerializeField] private float walkDecelerationSpeedLerp;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintSpeedLerp;
    [SerializeField, Range(0f, 1f)] private float walkInitSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(0f, 180f)] private float maxAngleTurn;
    [SerializeField, Range(0f, 90f)] private float maxSlopeAngle;
    [SerializeField] private AnimationCurve slopeSpeedUp;
    [SerializeField] private AnimationCurve slopeSpeedDown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        mapsMask = LayerMask.GetMask("Map");
        this.transform = base.transform;
    }

    private void Start()
    {
        currentAngle = Vector3.Angle(transform.forward, Vector3.right);
    }

    private void FixedUpdate()
    {
        UpdateState();

        HandleWalk();

        rb.linearVelocity = velocity;

        rb.rotation = Quaternion.Euler(0f, -currentAngle, 0f);

        print(currentAngle);
    }

    private void UpdateState()
    {
        currentAngle = Vector2.SignedAngle(Vector2.right, new Vector2(velocity.x, velocity.z));

        Physics.Raycast(transform.position, Vector3.down, out groundRaycast, groundRaycastLength, mapsMask);

        if (groundRaycast.collider != null)
        {
            Vector3 currentSpeed = velocity.sqrMagnitude > 1e-3f ? new Vector3(velocity.x, 0f, velocity.y) : transform.forward;
            slopeAngle = Useful.WrapAngle((Vector3.Angle(groundRaycast.normal, currentSpeed) - 90f) * Mathf.Deg2Rad) * Mathf.Rad2Deg;
            slopeAngle = slopeAngle > 90f ? slopeAngle - 360f : slopeAngle;
        }
        else
        {
            slopeAngle = 0f;
        }
    }

    #region Walk

    private void HandleWalk()
    {
        if (!isGrounded)
            return;

        Vector2 input = new Vector2(playerInput.x, playerInput.y);
        float targetAngle = Vector2.SignedAngle(Vector2.right, input);
        float newAngle = currentAngle;

        if(Mathf.Abs(targetAngle - newAngle) > maxAngleTurn)
        {
            newAngle = targetAngle;
        }
        else
        {
            newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        }

        float currentSpeed = velocity.magnitude;
        float targetSpeed = playerInput.isSprintPressed ? sprintSpeed : walkSpeed;

        if (playerInput.rawX == 0 && playerInput.rawY == 0)
        {
            targetSpeed = 0f;
        }

        float newSpeed = 0f;
        if (currentSpeed < walkInitSpeed * targetSpeed)
        {
            newSpeed = walkInitSpeed * targetSpeed * input.magnitude;
        }

        float delta = (playerInput.rawX == 0 && playerInput.rawY == 0) ? walkDecelerationSpeedLerp : 
            (playerInput.isSprintPressed ? sprintSpeedLerp : walkSpeedLerp);
        newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed * input.magnitude, delta * Time.fixedDeltaTime);

        Vector2 speed2D = Useful.Vector2FromAngle(newAngle * Mathf.Deg2Rad, newSpeed);
        Vector3 dir = new Vector3(speed2D.x, 0f, speed2D.y).normalized;
        dir = new Vector3(dir.x, Mathf.Tan(slopeAngle * Mathf.Deg2Rad), dir.z).normalized;
        velocity = dir * newSpeed;
    }

    #endregion

    #region Fall

    private void HandleFall()
    {
        if (isGrounded)
            return;


    }

    #endregion

    #region Gizmos/OnValidate

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if(!drawGizmos)
            return;

        this.transform = base.transform;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);
    }

    private void OnValidate()
    {
        this.transform = base.transform;
        walkSpeed = Mathf.Max(walkSpeed, 0f);
        sprintSpeed = Mathf.Max(sprintSpeed, 0f);
        walkSpeedLerp = Mathf.Max(walkSpeedLerp, 0f);
        sprintSpeedLerp = Mathf.Max(sprintSpeedLerp, 0f);
        walkDecelerationSpeedLerp = Mathf.Max(walkDecelerationSpeedLerp, 0f);
        rotationSpeed = Mathf.Max(rotationSpeed, 0f);
    }

#endif

#endregion
}
