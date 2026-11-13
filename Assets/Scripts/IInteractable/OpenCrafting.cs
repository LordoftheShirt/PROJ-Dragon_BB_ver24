using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class OpenCrafting : MonoBehaviour, IInteractable
{
    [SerializeField] private CraftingUI craftingUI;
    public void Interact()
    {
        InputMapManager.Instance.DisablePlayerMap();
        craftingUI.Open();
    }

    public string GetDescription()
    {
        return "Craft";
    }
}
