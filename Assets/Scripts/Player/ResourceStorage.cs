using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : Singleton<ResourceStorage>
{
    private ResourceDictionary _resources = new();
}
