using UnityEngine;

public class FlagMarker : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        Destroy(gameObject);
    }

    public string GetDescription()
    {
        return "Recollect Flag";
    }
}
