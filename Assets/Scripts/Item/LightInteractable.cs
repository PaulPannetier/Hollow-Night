using UnityEngine;

public abstract class LightInteractable : MonoBehaviour
{
    private int interractCounter = 0;
    protected bool isActive => interractCounter > 0;

    public virtual void BeginInteract(GameObject lighter)
    {
        interractCounter++;
    }

    public virtual void EndInteract(GameObject lighter)
    {
        interractCounter--;
    }
}
