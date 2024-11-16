using UnityEngine;

public class TorchSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Light torchLight;
    [SerializeField] private Light spottedLight;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        torchLight.enabled = false;
        spottedLight.enabled = false;
    }

    void Update()
    {
        if (playerInput.isTorchPressed)
        {
            torchLight.enabled = true;
            spottedLight.enabled = true;
        }
        else if (playerInput.isTorchUp)
        {
            torchLight.enabled = false;
            spottedLight.enabled = false;
        }
    }
}
