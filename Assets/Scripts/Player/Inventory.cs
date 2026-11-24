using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the players Resources, Items and unlocked Blueprints
/// </summary>
[LoadOnInit]
public class Inventory : SingletonPersistent<Inventory>
{
    public static Dictionary<ResourceType, int> Resources => _resources.Dictionary;
    private static readonly ResourceDictionary _resources = new();
    public static readonly Dictionary<Item, int> Items = new();
    public static List<Blueprint> BlueprintsUnlocked => Instance.blueprintsUnlocked;
    [SerializeField] private List<SerializableItemAndAmount<ResourceType>> startingResources = new();
    [SerializeField] private List<SerializableItemAndAmount<Item>> startingItems = new();
    [SerializeField] private List<Blueprint> blueprintsUnlocked = new();

    #region Resources
    public static ChangeDelegate<ResourceType> OnResourceChanged
    {
        get => _resources.OnChanged;
        set => _resources.OnChanged = value;
    }
    public void AddResource(ResourceType resourceType, int change) => _resources.AddResource(resourceType, change);
    public void RemoveResource(ResourceType resourceType, int change) => _resources.RemoveResource(resourceType, change);
    public int GetResource(ResourceType resourceType) => _resources.GetResource(resourceType);
    public void SetResource(ResourceType resourceType, int count) => _resources.SetResource(resourceType, count);
    #endregion

    #region Item
    public static ChangeDelegate<Item> OnItemChanged { get; set; }
    /// <summary>
    /// Add an item to the players inventory. Returns true if the item was successfully added to the inventory (will be false if the player has reached the max stack size)
    /// </summary>
    public bool AddItem(Item item, int change = 1)
    {
        if (change <= 0)
        {
            Debug.LogWarning($"Attempted to add {change} of \"{item.Name}\" to inventory. Count must be greater than 0.");
            return false;
        }

        if (Items.ContainsKey(item))
        {
            if (!item.UnlimitedStackSize && Items[item] >= item.MaxStackSize)
            {
                return false;
            }

            int count = Items[item] + change;

            // Clamp if necessary
            if (!item.UnlimitedStackSize && count > item.MaxStackSize)
            {
                count = item.MaxStackSize;
            }

            Items[item] = count;

            OnItemChanged?.Invoke(item, count, change);

            return true;
        }
        else
        {
            Items[item] = change;
            OnItemChanged?.Invoke(item, change, change);
            return true;
        }
    }

    /// <summary>
    /// Removes an item from the players inventory. Returns true if the item was successfully removed from the inventory (will be false if the player doesn't have any of the item)
    /// </summary>
    public bool RemoveItem(Item item, int change = 1)
    {
        if (change <= 0)
        {
            Debug.LogWarning($"Attempted to remove {change} of \"{item.Name}\" from inventory. Count must be greater than 0.");
            return false;
        }

        if (Items.ContainsKey(item))
        {
            int count = Items[item] - change;

            if (count <= 0)
            {
                count = 0;
                Items.Remove(item);
            }
            else
            {
                Items[item] = count;
            }

            OnItemChanged?.Invoke(item, count, change);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes ALL instances of an item from the players inventory. Returns true if the item was successfully removed from the inventory (will be false if the player doesn't have any of the item)
    /// </summary>
    public bool RemoveAllItem(Item item)
    {
        if (Items.TryGetValue(item, out int count))
        {
            Items.Remove(item);
            OnItemChanged?.Invoke(item, 0, -count);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes ALL instances of ALL items from the players inventory.
    /// </summary>
    public void ClearAllItems()
    {
        foreach (var pair in Items)
        {
            OnItemChanged?.Invoke(pair.Key, 0, -pair.Value);
        }

        Items.Clear();
    }

    public void SetItem(Item item, int count)
    {
        Items.TryGetValue(item, out int oldCount);
        int newCount;

        if (count <= 0)
        {
            newCount = 0;
            Items.Remove(item);
        }
        else
        {
            newCount = item.UnlimitedStackSize ? count : Mathf.Min(count, item.MaxStackSize);
            Items[item] = newCount;
        }

        OnItemChanged?.Invoke(item, newCount, newCount - oldCount);
    }

    /// <summary>
    /// Will return true as long as the player has at least 1 of the item.
    /// </summary>
    public bool HasItem(Item item)
    {
        if (item == null) return false;
        return Items.ContainsKey(item);
    }

    /// <summary>
    /// Will return true as long as the player has the amount of the item specified
    /// </summary>
    public bool HasAmountOfItem(Item item, int count)
    {
        if (item == null) return false;
        return Items.ContainsKey(item) && Items[item] >= count;
    }

    public int GetItem(Item item)
    {
        if (item == null) return 0;
        if (Items.TryGetValue(item, out int result))
        {
            return result;
        }

        return 0;
    }
    #endregion

    #region Blueprints
    public static Action<Blueprint> OnUnlockBlueprint { get; set; }
    public void UnlockBlueprint(Blueprint blueprint)
    {
        if (HasBlueprint(blueprint))
        {
            Debug.Log($"Blueprint \"{blueprint.name}\" is already unlocked.");
            return;
        }

        blueprintsUnlocked.Add(blueprint);
        Debug.Log($"Blueprint \"{blueprint.name}\" unlocked!");

        OnUnlockBlueprint?.Invoke(blueprint);
    }

    public static Action<Blueprint> OnLockBlueprint { get; set; }
    public void LockBlueprint(Blueprint blueprint)
    {
        if (!HasBlueprint(blueprint))
        {
            Debug.Log($"Blueprint \"{blueprint.name}\" is not unlocked.");
            return;
        }

        blueprintsUnlocked.Remove(blueprint);
        Debug.Log($"Blueprint \"{blueprint.name}\" locked!");

        OnLockBlueprint?.Invoke(blueprint);
    }

    public bool HasBlueprint(Blueprint blueprint)
    {
        return blueprintsUnlocked.Contains(blueprint);
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            Resources[type] = 0;
        }

        foreach (SerializableItemAndAmount<ResourceType> pair in startingResources)
        {
            ResourceType resource = pair.Item;
            int count = pair.Count;

            Resources[resource] = count;
        }

        foreach (SerializableItemAndAmount<Item> pair in startingItems)
        {
            Item item = pair.Item;
            int count = pair.Count;

            AddItem(item, count);
        }
    }

    [System.Serializable]
    public class SerializableItemAndAmount<T>
    {
        public T Item => item;
        public int Count => count;
        [SerializeField] private T item;
        [SerializeField] private int count;
    }
}
