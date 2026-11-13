using System;
using UnityEngine;

public class ResourceTypes : MonoBehaviour
{
    [SerializeField] private string resourceName;


    //private int resourceAmount;

    //protected virtual void Start()
    //{
    //    resourceAmount = 0;
    //}

    public string GetResourceName()
    {
        return resourceName;
    }

    //    public void AddResourceAmmount(int increaseAmount)
    //    {
    //        resourceAmount += increaseAmount;
    //    }

    //    public int GetResourceAmount()
    //    {
    //        return resourceAmount;
    //    }
}
