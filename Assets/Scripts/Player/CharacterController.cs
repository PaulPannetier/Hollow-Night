using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 velocity;
    private LayerMask mapsMask;
    private new Transform transform;
    private RaycastHit groundRaycast;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        mapsMask = LayerMask.GetMask("Map");
        this.transform = base.transform;
    }

    private void FixedUpdate()
    {
        UpdateState();

        HandleWalk();

        HandleSlope();

        rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.y);
    }

    private void UpdateState()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycast, float.MaxValue, mapsMask);
    }

    #region Walk

    private void HandleWalk()
    {
        if (playerInput.rawX == 0 && playerInput.rawY == 0)
        {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, walkDecelerationSpeedLerp * Time.fixedDeltaTime);
            return;
        }

        Vector2 input = new Vector2(playerInput.x, playerInput.y);
        float currentAngle = Vector2.SignedAngle(Vector2.right, velocity);
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
        float newSpeed = 0f;

        if (currentSpeed < walkInitSpeed * targetSpeed)
        {
            newSpeed = walkInitSpeed * targetSpeed * input.magnitude;
        }

        float delta = playerInput.isSprintPressed ? sprintSpeedLerp : walkSpeedLerp;
        newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed * input.magnitude, delta * Time.fixedDeltaTime);

        velocity = Useful.Vector2FromAngle(newAngle * Mathf.Deg2Rad, newSpeed);
    }

    #endregion

    #region Slode

    private void HandleSlope()
    {
        //if()
        //{
        //    float slopeAngleRight = Useful.WrapAngle((Vector2.Angle(Vector2.right, raycast.normal) - 90f) * Mathf.Deg2Rad);

        //}

    }

    #endregion

    #region OnValidate

    private void OnValidate()
    {
        walkSpeed = Mathf.Max(walkSpeed, 0f);
        sprintSpeed = Mathf.Max(sprintSpeed, 0f);
        walkSpeedLerp = Mathf.Max(walkSpeedLerp, 0f);
        sprintSpeedLerp = Mathf.Max(sprintSpeedLerp, 0f);
        walkDecelerationSpeedLerp = Mathf.Max(walkDecelerationSpeedLerp, 0f);
        rotationSpeed = Mathf.Max(rotationSpeed, 0f);
    }

    #endregion
}
