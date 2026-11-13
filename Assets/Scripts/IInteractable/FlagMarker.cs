using UnityEngine;
using UnityEngine.Events;

public class FlagMarker : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        ConsumablesManager.Instance.AddFlag();
        Destroy(gameObject);
    }

    public string GetDescription()
    {
        return "Recollect Flag";
    }
}
