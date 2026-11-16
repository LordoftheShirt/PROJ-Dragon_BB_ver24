using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class OpenCrafting : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        gameObject.GetComponent<SquashAndStretch>().Animate();
        InputMapManager.Instance.DisablePlayerMap();

        // Open crafting Menu here.
        CraftingUI.Instance.Open();
    }

    public string GetDescription()
    {
        return "Craft";
    }
}
