//using Mono.Cecil;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IInteractable
{
    [SerializeField] private int resourceMin = 1;
    [SerializeField] private int resourceMax = 5;
    [SerializeField] private int maxResourceAmountPerCluster = 10;

    private int resourceAmount = 100;
    private ResourceType resourceType;

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
        Debug.Log("Interacted with resource!");
        //kan ändra ettan till en stat på spelaren
        Inventory.Instance.AddResource(resourceType, CollectResource(1));
        gameObject.GetComponent<SquashAndStretch>().Animate();
        if (resourceAmount <= 0) { Destroy(gameObject,0.4f); }       
    }

    public virtual ResourceType GetResourceType() { return resourceType; }

    public virtual string GetDescription() { return "Collect Resource " + resourceType.GetDisplayString(); }

    public virtual int GetResourceMin() { return resourceMin; }

    public virtual int GetResourceMax() { return resourceMax; }

    public int GetMaxResourceAmountPerCluster() { return maxResourceAmountPerCluster; }

}
