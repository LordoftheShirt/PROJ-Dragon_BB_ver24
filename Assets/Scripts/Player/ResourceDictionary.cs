using System.Collections.Generic;
using UnityEngine;

public class ResourceDictionary
{
    public readonly Dictionary<ResourceType, int> Dictionary = new();

    public ChangeDelegate<ResourceType> OnChanged { get; set; }
    public void AddResource(ResourceType resourceType, int change)
    {
        if (change <= 0)
        {
            Debug.LogWarning($"Attempted to add {change} of \"{resourceType.GetDisplayString()}\" to inventory. Count must be greater than 0.");
            return;
        }

        if (Dictionary.ContainsKey(resourceType))
        {
            Dictionary[resourceType] += change;
        }
        else
        {
            Dictionary[resourceType] = change;
        }

        OnChanged?.Invoke(resourceType, Dictionary[resourceType], change);
    }

    public int RemoveResource(ResourceType resourceType, int change)
    {
        if (change <= 0)
        {
            Debug.LogWarning($"Attempted to remove {change} of \"{resourceType.GetDisplayString()}\" from inventory. Count must be greater than 0.");
            return 0;
        }

        if (Dictionary.ContainsKey(resourceType))
        {
            int countBefore = Dictionary[resourceType];
            if (countBefore < change)
            {
                change = countBefore;
            }

            Dictionary[resourceType] -= change;

            // Cannot fall below 0
            /*
            if (Dictionary[resourceType] <= 0)
            {
                Dictionary[resourceType] = 0;
            }
            */
        }
        else
        {
            Dictionary[resourceType] = 0;
        }

        OnChanged?.Invoke(resourceType, Dictionary[resourceType], change);

        return change;
    }

    public int GetResource(ResourceType resourceType)
    {
        return Dictionary[resourceType];
    }

    public void SetResource(ResourceType resourceType, int count)
    {
        int oldCount = Dictionary[resourceType];
        int newCount = Mathf.Max(count, 0);
        Dictionary[resourceType] = newCount;

        OnChanged?.Invoke(resourceType, newCount, newCount - oldCount);
    }

    public ResourceDictionary()
    {
        foreach (ResourceType type in ResourceTypeUtil.GetAllResourceTypes())
        {
            Dictionary[type] = 0;
        }
    }
}
