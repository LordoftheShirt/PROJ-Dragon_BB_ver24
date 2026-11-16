using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the players Resources, Items and unlocked Blueprints
/// </summary>
[LoadOnInit]
public class Inventory : SingletonPersistent<Inventory>
{
    public static readonly Dictionary<ResourceType, int> Resources = new();
    public static readonly Dictionary<Item, int> Items = new();
    public static List<Blueprint> BlueprintsUnlocked => Instance.blueprintsUnlocked;
    [SerializeField] private List<SerializableItemAndAmount<ResourceType>> startingResources = new();
    [SerializeField] private List<SerializableItemAndAmount<Item>> startingItems = new();
    [SerializeField] private List<Blueprint> blueprintsUnlocked = new();

    #region Resources
    public void AddResource(ResourceType resourceType, int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"Attempted to add {count} of \"{resourceType.GetDisplayString()}\" to inventory. Count must be greater than 0.");
            return;
        }

        if (Resources.ContainsKey(resourceType))
        {
            Resources[resourceType] += count;
        }
        else
        {
            Resources[resourceType] = count;
        }
    }

    public void RemoveResource(ResourceType resourceType, int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"Attempted to remove {count} of \"{resourceType.GetDisplayString()}\" from inventory. Count must be greater than 0.");
            return;
        }

        if (Resources.ContainsKey(resourceType))
        {
            Resources[resourceType] -= count;

            // Cannot fall below 0
            if (Resources[resourceType] <= 0)
            {
                Resources[resourceType] = 0;
            }
        }
    }

    public int GetResourceCount(ResourceType resourceType)
    {
        return Resources[resourceType];
    }

    public void SetResourceCount(ResourceType resourceType, int count)
    {
        Resources[resourceType] = Mathf.Max(count, 0);
    }
    #endregion

    #region Item
    /// <summary>
    /// Add an item to the players inventory. Returns true if the item was successfully added to the inventory (will be false if the player has reached the max stack size)
    /// </summary>
    public bool AddItem(Item item, int count = 1)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"Attempted to add {count} of \"{item.Name}\" to inventory. Count must be greater than 0.");
            return false;
        }

        if (Items.ContainsKey(item))
        {
            if (!item.UnlimitedStackSize && Items[item] >= item.MaxStackSize)
            {
                return false;
            }

            Items[item] += count;

            // Clamp if necessary
            if (!item.UnlimitedStackSize && Items[item] > item.MaxStackSize)
            {
                Items[item] = item.MaxStackSize;
            }

            return true;
        }
        else
        {
            Items[item] = count;
            return true;
        }
    }

    /// <summary>
    /// Removes an item from the players inventory. Returns true if the item was successfully removed from the inventory (will be false if the player doesn't have any of the item)
    /// </summary>
    public bool RemoveItem(Item item, int count = 1)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"Attempted to remove {count} of \"{item.Name}\" from inventory. Count must be greater than 0.");
            return false;
        }

        if (Items.ContainsKey(item))
        {
            Items[item] -= count;
            if (Items[item] <= 0)
            {
                Items.Remove(item);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes ALL instances of an item from the players inventory. Returns true if the item was successfully removed from the inventory (will be false if the player doesn't have any of the item)
    /// </summary>
    public bool RemoveAllItem(Item item)
    {
        if (Items.ContainsKey(item))
        {
            Items.Remove(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes ALL instances of ALL items from the players inventory.
    /// </summary>
    public void ClearAllItems()
    {
        Items.Clear();
    }

    public void SetItem(Item item, int count)
    {
        if (count <= 0)
        {
            Items.Remove(item);
        }
        else
        {
            Items[item] = item.UnlimitedStackSize ? count : Mathf.Min(count, item.MaxStackSize);
        }
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

    public int GetItemCount(Item item)
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
    public void UnlockBlueprint(Blueprint blueprint)
    {
        if (HasBlueprint(blueprint))
        {
            Debug.Log($"Blueprint \"{blueprint.name}\" is already unlocked.");
            return;
        }

        blueprintsUnlocked.Add(blueprint);
        Debug.Log($"Blueprint \"{blueprint.name}\" unlocked!");
    }

    public void LockBlueprint(Blueprint blueprint)
    {
        if (!HasBlueprint(blueprint))
        {
            Debug.Log($"Blueprint \"{blueprint.name}\" is not unlocked.");
            return;
        }
        blueprintsUnlocked.Remove(blueprint);
        Debug.Log($"Blueprint \"{blueprint.name}\" locked!");
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
