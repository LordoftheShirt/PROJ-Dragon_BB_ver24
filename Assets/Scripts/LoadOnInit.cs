using System;
using System.Reflection;
using UnityEngine;

// Put this on a script to load it from the resources folder on initialize. Ex. put on a persistent singleton script to load it on start without needing to manually instantiate it.
public class LoadOnInitAttribute : Attribute
{
    public string Path { get; private set; }
}

public static class LoadOnInitManager
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        // Loop through every type in every assembly and check if it has the LoadOnInitAttribute. If it does, load it from resources.
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                LoadOnInitAttribute attr = type.GetCustomAttribute<LoadOnInitAttribute>();

                if (attr != null)
                {
                    string path = string.IsNullOrEmpty(attr.Path) ? type.Name : attr.Path;

                    GameObject resource = Resources.Load<GameObject>(path);

                    if (resource != null)
                    {
                        GameObject obj = GameObject.Instantiate(resource);
                        obj.name = resource.name;
                    }
                }
            }
        }
    }
}
