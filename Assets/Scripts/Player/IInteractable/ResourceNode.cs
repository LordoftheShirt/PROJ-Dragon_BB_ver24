using Mono.Cecil;
using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IInteractable
{
    [SerializeField] private int resourceAmount = 100;
    [SerializeField] private ResourceType resourceType;

    private void Start()
    {
        ReferenceManager.Instance.mapManager.AddMapObject(gameObject);
    }

    private void OnDestroy()
    {
        //ReferenceManager.Instance.mapManager.RemoveMapObject(gameObject);
    }

    public void SetResourceType(ResourceType newResourceType, int newResourceAmount)
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
        if (resourceAmount <= 0) { Destroy(gameObject); }       
    }

    public virtual ResourceType GetResourceType()
    {
        return resourceType;
    }

    public virtual string GetDescription()
    {
        return "Collect Resource";
    }
}
