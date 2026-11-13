using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[LoadOnInit]
public class AssetStorage : SingletonPersistent<AssetStorage>
{
    // The asset types to store. You can add more types here if needed as long as they are ScriptableObjects
    public static readonly Type[] AssetTypes = new Type[]
    {
        typeof(Item),
        typeof(Recipe)
    };

    public static Dictionary<Type, Object[]> AssetDictionary { get; private set; } = new Dictionary<Type, Object[]>();
    public static Object[] Assets => Instance.assets;
    [SerializeField] private Object[] assets;

    protected override void Awake()
    {
        base.Awake();

        // Populate temp dictionary
        Dictionary<Type, List<Object>> tempDictionary = new Dictionary<Type, List<Object>>();
        foreach (Object asset in Assets)
        {
            Type type = asset.GetType();

            if (!tempDictionary.ContainsKey(type))
            {
                tempDictionary[type] = new();
            }
            tempDictionary[type].Add(asset);
        }

        // Convert lists to arrays and store in the final dictionary
        foreach (var kvp in tempDictionary)
        {
            AssetDictionary[kvp.Key] = kvp.Value.ToArray();
        }
    }

    public static T[] GetAssetsOfType<T>() where T : Object
    {
        Type type = typeof(T);
        if (AssetDictionary.TryGetValue(type, out Object[] assets))
        {
            return Array.ConvertAll(assets, item => (T)item);
        }
        return Array.Empty<T>();
    }

    public static Object[] GetAssetsOfType(Type type)
    {
        if (AssetDictionary.TryGetValue(type, out Object[] assets))
        {
            return assets;
        }
        return Array.Empty<Object>();
    }
}