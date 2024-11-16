using UnityEngine;

public class Halo : MonoBehaviour
{
    private bool isInHead;
    private Transform hat;
    private float timePutOnHead, lastTimeFall;

    private LayerMask playerMask, mapMask;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float radius;
    [SerializeField] private float duration;
    [SerializeField] private float distanceFromGround;

    private void Awake()
    {
        playerMask = LayerMask.GetMask("Player");
        mapMask = LayerMask.GetMask("Map");
        lastTimeFall = float.MinValue;
    }

    private void Start()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycast, float.MaxValue, mapMask))
        {
            transform.position = raycast.point + distanceFromGround * Vector3.up;
        }
    }

    private void Update()
    {
        if (isInHead)
        {
            HandleHead();
            return;
        }

        if(Time.time - lastTimeFall > 1f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + offset, radius, playerMask);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    hat = collider.gameObject.GetComponent<PlayerData>().hatTransform;
                    isInHead = true;
                    timePutOnHead = Time.time;
                    break;
                }
            }
        }
    }

    private void HandleHead()
    {
        transform.position = hat.position;

        if (Time.time - timePutOnHead > duration)
        {
            timePutOnHead = -10f;
            hat = null;
            isInHead = false;
            lastTimeFall = Time.time;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycast, float.MaxValue, mapMask))
            {
                transform.position = raycast.point + distanceFromGround * Vector3.up;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    #region OnValidate/Gizmos

#if UNITY_EDITOR

    private void OnValidate()
    {
        radius = Mathf.Max(radius, 0f);
        distanceFromGround = Mathf.Max(distanceFromGround, 0f);
        duration = Mathf.Max(duration, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, radius);
    }

#endif

    #endregion
}
