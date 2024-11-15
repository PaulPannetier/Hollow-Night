using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
