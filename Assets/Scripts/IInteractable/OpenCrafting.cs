using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class OpenCrafting : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        InputMapManager.Instance.DisablePlayerMap();

        // Open crafting Menu here.
    }

    public string GetDescription()
    {
        return "Craft";
    }
}
