using Mono.Cecil;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IInteractable
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

    public void SetResourceType(ResourceTypes newResourceType, int newResourceAmount)
    {
        resourceType = newResourceType;
        resourceAmount = newResourceAmount;
    }

    public int CollectResource(int amount)
    {
        int collectedAmount = Mathf.Min(amount, resourceAmount);
        resourceAmount -= collectedAmount;
        //resourceType.AddResourceAmmount(collectedAmount); // flytta till spelaren
        return collectedAmount;
    }

    public virtual void Interact()
    {
        // Implementation for interacting with firewood
        Debug.Log("Interacted with resource!");
        CollectResource(1);
    }

    public virtual string GetResourceType()
    {
        return resourceType.GetResourceName();
    }

    public virtual string GetDescription()
    {
        return "Collect Resource";
    }
}
