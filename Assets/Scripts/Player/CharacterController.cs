using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PlayerInput playerInput;
    private Vector3 velocity;
    private LayerMask mapsMask;
    private new Transform transform;
    private RaycastHit groundRaycast;
    private float slopeAngle;
    private float currentAngle;
    private bool isGrounded => groundRaycast.collider != null;
    private float lastTimeSpawnSprintFX;

#if UNITY_EDITOR
    [SerializeField] private bool drawGizmos;
#endif

    [Header("Detection")]
    [SerializeField] private float groundCastLength;

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
    [SerializeField] private AnimationCurve slopeUpSpeedMultiplier;
    [SerializeField] private AnimationCurve slopeDownSpeedMultiplier;

    [Header("Fall")]
    [SerializeField] private float fallSpeed;
    [SerializeField] private float fallDecelerationSpeedLerp;
    [SerializeField] private float fallSpeedLerp;

    [Header("FX")]
    [SerializeField] private GameObject[] sprintFX;
    [SerializeField] private float delayBetween2sprintFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mapsMask = LayerMask.GetMask("Map");
        this.transform = base.transform;
        lastTimeSpawnSprintFX = -10f;
    }

    private void Start()
    {
        currentAngle = Vector3.Angle(transform.forward, Vector3.right) - 90f;
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, float.MaxValue, mapsMask);
        rb.MovePosition(raycastHit.point + Vector3.up * capsuleCollider.height);
    }

    private void FixedUpdate()
    {
        UpdateState();

        HandleWalk();

        HandleFall();

        rb.linearVelocity = velocity;

        rb.rotation = Quaternion.Euler(0f, -currentAngle, 0f);
    }

    private void UpdateState()
    {
        currentAngle = Vector2.SignedAngle(Vector2.up, new Vector2(velocity.x, velocity.z));

        Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out RaycastHit groundRaycast, groundCastLength, mapsMask);
        if(groundRaycast.collider != null && !isGrounded)
        {
            velocity.y = 0f;
        }
        this.groundRaycast = groundRaycast;
        //Physics.Raycast(transform.position, Vector3.down, out groundRaycast, groundCastLength, mapsMask);

        if (groundRaycast.collider != null)
        {
            Vector3 currentSpeed = velocity.sqrMagnitude > 1e-3f ? new Vector3(velocity.x, 0f, velocity.z) : transform.forward;
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
        float targetAngle = Vector2.SignedAngle(Vector2.up, input);
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
        float slopePercent = Mathf.Abs(slopeAngle) / maxSlopeAngle;
        float speedCoeff = slopeAngle > 0f ? slopeUpSpeedMultiplier.Evaluate(slopePercent) : slopeDownSpeedMultiplier.Evaluate(slopePercent);
        float targetSpeed = (playerInput.isSprintPressed ? sprintSpeed : walkSpeed) * speedCoeff;

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

        Vector2 speed2D = Useful.Vector2FromAngle((newAngle + 90f) * Mathf.Deg2Rad, newSpeed);
        Vector3 dir = new Vector3(speed2D.x, 0f, speed2D.y).normalized;
        dir = new Vector3(dir.x, Mathf.Tan(slopeAngle * Mathf.Deg2Rad), dir.z).normalized;
        velocity = dir * newSpeed;

        //FX
        if(!(playerInput.rawX == 0 && playerInput.rawY == 0) && playerInput.isSprintPressed)
        {
            if(Time.time - lastTimeSpawnSprintFX > delayBetween2sprintFX && LevelManager.instance.isNight)
            {
                Instantiate(sprintFX.GetRandom(), transform.position + Vector3.down * capsuleCollider.height, Quaternion.identity, transform);
                lastTimeSpawnSprintFX = Time.time;
            }
        }
    }

    #endregion

    #region Fall

    private void HandleFall()
    {
        if (isGrounded)
            return;

        velocity = new Vector3(Mathf.MoveTowards(velocity.x, 0f, fallDecelerationSpeedLerp * Time.fixedDeltaTime),
            Mathf.MoveTowards(velocity.y, -fallSpeed, fallSpeedLerp * Time.fixedDeltaTime),
            Mathf.MoveTowards(velocity.z, 0f, fallDecelerationSpeedLerp * Time.fixedDeltaTime));
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
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCastLength);
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
        fallSpeed = Mathf.Max(fallSpeed, 0f);
        fallDecelerationSpeedLerp = Mathf.Max(fallDecelerationSpeedLerp, 0f);
        fallSpeedLerp = Mathf.Max(fallSpeedLerp, 0f);
    }

#endif

#endregion
}
