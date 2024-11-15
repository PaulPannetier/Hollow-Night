using UnityEngine;

public class TorchSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Light torchLight;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchLight.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.isTorchPressed)
        {
            torchLight.enabled = true;
            Debug.Log("pressed");
        }
        else if (playerInput.isTorchUp)
        {
            torchLight.enabled = false;
        }
    }
}
