using System;
using UnityEngine;

public class PlayerResourceManager : MonoBehaviour
{
    private int[] resources = new int[(int)ResourceType.NumberOfResourceTypes];

    public void AddResource(int amount, ResourceType resource) 
    {
        resources[(int)resource] += amount;
    }
    public void RemoveResource(int amount, ResourceType resource)
    {
        resources[(int)resource] -= amount;
    }
    public int GetResourceCount(ResourceType resource)
    { 
        return resources[(int)resource]; 
    }

}
