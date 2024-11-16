using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void OnPlayerLightUp(GameObject otherPlayer)
    {
        characterController.OnLightUp();
    }

    public void OnPlayerEndLightUp(GameObject otherPlayer)
    {
        characterController.OnEndLightUp();
    }
}
