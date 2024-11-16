using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private CharacterController characterController;
    private TorchSystem torchSystem;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        torchSystem = GetComponent<TorchSystem>();
    }

    public void OnPlayerLightUp(GameObject otherPlayer)
    {
        characterController.OnLightUp();
    }

    public void OnPlayerEndLightUp(GameObject otherPlayer)
    {
        characterController.OnEndLightUp();
    }

    public void OnPlayerPutHat()
    {
        print("OnPutHat");
        torchSystem.OnPutHat();
    }

    public void OnPlayerReleaseHat()
    {
        print("OnPutHat");
        torchSystem.OnRealeaseHat();
    }
}
