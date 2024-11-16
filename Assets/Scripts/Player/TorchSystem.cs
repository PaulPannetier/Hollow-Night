using Unity.VisualScripting;
using UnityEngine;

public class TorchSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Light torchLight;
    [SerializeField] private Light spottedLight;
    private bool isNight => LevelManager.instance.isNight;
    private LayerMask playerAndWallMask;
    private bool test;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAndWallMask = LayerMask.GetMask("Map", "Player");
    }

    void Start()
    {
        torchLight.enabled = false;
        spottedLight.enabled = false;
    }

    void Update()
    {
        if (playerInput.isTorchPressed && isNight)
        {
            torchLight.enabled = true;
            spottedLight.enabled = true;
            RaycastHit hitInfo;
            Physics.Raycast(transform.position, transform.forward, out hitInfo, 4f, playerAndWallMask);
            Debug.DrawLine(transform.position, transform.position + 4*transform.forward, Color.green);
            if (hitInfo.collider != null)
            {
                Debug.Log(hitInfo.rigidbody.gameObject);
                if(hitInfo.rigidbody.gameObject != null)
                {
                    Debug.Log("touché");
                    LevelManager.instance.DestroyPlayer(hitInfo.rigidbody.gameObject.GetComponent<PlayerData>());
                }
            }
        }
        else if (playerInput.isTorchUp)
        {
            torchLight.enabled = false;
            spottedLight.enabled = false;
        }
        
    }
}
