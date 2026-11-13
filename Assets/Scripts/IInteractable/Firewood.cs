using Mono.Cecil;
using UnityEngine;

public class Firewood : MonoBehaviour, IInteractable
{
    [SerializeField] private int resourceAmount = 100;
    [SerializeField] private ResourceTypes resourceType;

    private void Start()
    {
        ReferenceManager.Instance.mapManager.AddMapObject(gameObject);
    }

    private void OnDestroy()
    {
        ReferenceManager.Instance.mapManager.RemoveMapObject(gameObject);
    }

    public ResourceTypes CollectResource(int amount)
    {
        int collectedAmount = Mathf.Min(amount, resourceAmount);
        resourceAmount -= collectedAmount;
        //resourceType.AddResourceAmmount(collectedAmount); // flytta till spelaren
        return resourceType;
    }

    public void Interact()
    {
        // Implementation for interacting with firewood
        Debug.Log("Interacted with firewood!");
        CollectResource(1);
    }

    public string GetDescription()
    {
        return "Collect Wood";
    }
}
