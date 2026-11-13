using UnityEngine;
using UnityEngine.Events;

public class OpenCrafting : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        // Disable Player ActionMap
        InputMapManager.Instance.DisablePlayerMap();
    }

    public string GetDescription()
    {
        return "Craft";
    }
}
