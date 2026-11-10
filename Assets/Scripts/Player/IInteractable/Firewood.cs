using UnityEngine;

public class Firewood : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // Implementation for interacting with firewood
        Debug.Log("Interacted with firewood!");
    }

    public string GetDescription()
    {
        return "Collect Wood";
    }
}
