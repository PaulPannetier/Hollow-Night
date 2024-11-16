using UnityEngine;

public abstract class LightInteractable : MonoBehaviour
{
    private int interractCounter = 0;
    protected bool isActive => interractCounter > 0;
    protected GameObject lastActivator;

    public virtual void BeginInteract(GameObject lighter)
    {
        print("Begin");
        interractCounter++;
        lastActivator = lighter;
    }

    public virtual void EndInteract(GameObject lighter)
    {
        print("end");
        interractCounter--;
    }
}
