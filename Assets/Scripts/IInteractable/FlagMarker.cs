using UnityEngine;
using UnityEngine.Events;

public class FlagMarker : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        if (ConsumablesManager.Instance.AddFlag())
        {
            // Successfully picked up flag
            Destroy(gameObject);
        }
    }

    public string GetDescription()
    {
        return "Recollect Flag";
    }
}
