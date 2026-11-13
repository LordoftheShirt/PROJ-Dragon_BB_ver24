using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the players Resources, Items and unlocked Blueprints
/// </summary>
[LoadOnInit]
public class Inventory : SingletonPersistent<Inventory>
{
    private Dictionary<ResourceType, int> _resources = new();
    private Dictionary<Item, int> _items = new();
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

        if (_resources.ContainsKey(resourceType))
        {
            _resources[resourceType] += count;
        }
        else
        {
            _resources[resourceType] = count;
        }
    }

    public void RemoveResource(ResourceType resourceType, int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"Attempted to remove {count} of \"{resourceType.GetDisplayString()}\" from inventory. Count must be greater than 0.");
            return;
        }

        if (_resources.ContainsKey(resourceType))
        {
            _resources[resourceType] -= count;

            // Cannot fall below 0
            if (_resources[resourceType] <= 0)
            {
                _resources[resourceType] = 0;
            }
        }
    }

    public int GetResourceCount(ResourceType resourceType)
    {
        return _resources[resourceType];
    }

    public void SetResourceCount(ResourceType resourceType, int count)
    {
        _resources[resourceType] = Mathf.Max(count, 0);
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

        if (_items.ContainsKey(item))
        {
            if (!item.UnlimitedStackSize && _items[item] >= item.MaxStackSize)
            {
                return false;
            }

            _items[item] += count;

            // Clamp if necessary
            if (!item.UnlimitedStackSize && _items[item] > item.MaxStackSize)
            {
                _items[item] = item.MaxStackSize;
            }

            return true;
        }
        else
        {
            _items[item] = count;
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

        if (_items.ContainsKey(item))
        {
            _items[item] -= count;
            if (_items[item] <= 0)
            {
                _items.Remove(item);
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
        if (_items.ContainsKey(item))
        {
            _items.Remove(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes ALL instances of ALL items from the players inventory.
    /// </summary>
    public void ClearAllItems()
    {
        _items.Clear();
    }

    public void SetItem(Item item, int count)
    {
        if (count <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            _items[item] = item.UnlimitedStackSize ? count : Mathf.Min(count, item.MaxStackSize);
        }
    }

    /// <summary>
    /// Will return true as long as the player has at least 1 of the item.
    /// </summary>
    public bool HasItem(Item item)
    {
        return _items.ContainsKey(item);
    }

    /// <summary>
    /// Will return true as long as the player has the amount of the item specified
    /// </summary>
    public bool HasAmountOfItem(Item item, int count)
    {
        return _items.ContainsKey(item) && _items[item] >= count;
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

        foreach (SerializableItemAndAmount<ResourceType> pair in startingResources)
        {
            ResourceType resource = pair.Item;
            int count = pair.Count;

            _resources[resource] = count;
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
