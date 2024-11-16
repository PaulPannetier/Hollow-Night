using UnityEngine;

public class TorchSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Light torchLight;
    [SerializeField] private Light spottedLight;
    private bool isNight => LevelManager.instance.isNight;

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
        if (playerInput.isTorchPressed && isNight)
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
