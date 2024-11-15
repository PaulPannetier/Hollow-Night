using UnityEngine;

public class TorchSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Light torchLight;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        torchLight.enabled = false;
    }

    void Update()
    {
        if (playerInput.isTorchPressed)
        {
            torchLight.enabled = true;
        }
        else if (playerInput.isTorchUp)
        {
            torchLight.enabled = false;
        }
    }
}
