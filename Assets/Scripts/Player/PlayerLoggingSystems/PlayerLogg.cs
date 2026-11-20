using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class PlayerLogg : ScriptableObject
{
    private string loggName;
    private string loggDescription;

    //Test inputs:
    private ResourceType[] resourceType;
    private int[] reosurceAmount;
    //End of test inputs

    private NativeHashMap<int, int> resourceRecuierment;

    public void InstantiateTest()
    {
        AddResourceRequierment(resourceType, reosurceAmount);
    }

    public void AddResourceRequierment(ResourceType[] resourceTypes, int[] reosurceAmolunts)
    {
        foreach (ResourceType type in resourceTypes)
        {
            foreach (int amount in reosurceAmolunts)
                resourceRecuierment.Add((int)type, amount);
        }
    }

    public string GetLoggName() { return loggName; }
    public string GetLoggDescription() { return loggDescription; }

    public NativeHashMap<int, int> GetResourceReqierments() { return resourceRecuierment; }


}
