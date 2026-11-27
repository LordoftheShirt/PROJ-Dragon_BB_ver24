using DebugTools;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the players Resources, Items and unlocked Blueprints
/// </summary>
[LoadOnInit]
public class Inventory : SingletonPersistent<Inventory>
{
    public static Dictionary<ResourceType, int> TemporaryResources => _temporaryResources.Dictionary;
    public static Dictionary<ResourceType, int> PermanentResources => _permanentResources.Dictionary;
    private static readonly ResourceDictionary _temporaryResources = new();
    private static readonly ResourceDictionary _permanentResources = new();
    public static readonly Dictionary<Item, int> Items = new();
    public static List<Blueprint> BlueprintsUnlocked => Instance.blueprintsUnlocked;
    [SerializeField] private List<SerializableItemAndAmount<Item>> startingItems = new();
    [SerializeField] private List<Blueprint> blueprintsUnlocked = new();

    #region Resources
    public static ChangeDelegate<ResourceType> OnResourceChanged { get; set; }
    public void AddResource(ResourceType resourceType, int change)
    {
        _temporaryResources.AddResource(resourceType, change);

        OnResourceChanged?.Invoke(resourceType, GetResource(resourceType), change);
    }
    public void RemoveResource(ResourceType resourceType, int change)
    {
        int temporaryResourcesChange = _temporaryResources.RemoveResource(resourceType, change);

        int permanentChange = change - temporaryResourcesChange;

        if (permanentChange > 0) _permanentResources.RemoveResource(resourceType, permanentChange);

        OnResourceChanged?.Invoke(resourceType, GetResource(resourceType), change);
    }
    public int GetResource(ResourceType resourceType) => _temporaryResources.GetResource(resourceType) + _permanentResources.GetResource(resourceType);
    private void SetResource(ResourceDictionary dictionary, ResourceDictionary otherDictionary, ResourceType resourceType, int count)
    {
        int oldCount = dictionary.GetResource(resourceType);
        int newCount = Mathf.Max(count, 0);

        dictionary.SetResource(resourceType, newCount);

        OnResourceChanged?.Invoke(resourceType, otherDictionary.GetResource(resourceType) + newCount, newCount - oldCount);
    }
    public void SetTemporaryResource(ResourceType resourceType, int count) => SetResource(_temporaryResources, _permanentResources, resourceType, count);
    public void SetPermanentResource(ResourceType resourceType, int count) => SetResource(_permanentResources, _temporaryResources, resourceType, count);

    public void RemoveAllTemporaryResources()
    {
        foreach (var type in ResourceTypeUtil.GetAllResourceTypes())
        {
            SetTemporaryResource(type, 0);
        }
    }

    public void RemoveAllPermanentResources()
    {
        foreach (var type in ResourceTypeUtil.GetAllResourceTypes())
        {
            SetPermanentResource(type, 0);
        }
    }

    public void ConvertTemporaryResourcesToPermanent()
    {
        List<ResourceType> types = new();
        foreach (var pair in TemporaryResources)
        {
            PermanentResources[pair.Key] += pair.Value;
            types.Add(pair.Key);
        }

        foreach (var type in types)
        {
            TemporaryResources[type] = 0;
        }

        // No amount was actually changed so don't bother with the invoking OnResourceChanged
    }
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

    public void SetItemAmount(Item item, int count)
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

    public int GetItemAmount(Item item)
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

        foreach (SerializableItemAndAmount<Item> pair in startingItems)
        {
            Item item = pair.Item;
            int count = pair.Count;

            AddItem(item, count);
        }
    }

    #region Resources Debug Commands
    [DebugCommand("Technically not infinite, just adds 9999 to every resource", DisplayName = "AddInfiniteResources")]
    public static void AddInfiniteResourcesDebug()
    {
        foreach (var type in ResourceTypeUtil.GetAllResourceTypes())
        {
            Instance.AddResource(type, 9999);
        }
    }

    [DebugCommand("Sets all resources to 0", DisplayName = "RemoveAllResources")]
    public static void RemoveAllResourcesDebug()
    {
        Instance.RemoveAllTemporaryResources();
        Instance.RemoveAllPermanentResources();
    }

    [DebugCommand(DisplayName = "AddResource")]
    public static void AddResourceDebug(string type, int amount)
    {
        ResourceType resourceType;

        if (Enum.TryParse(typeof(ResourceType), type, true, out object result))
        {
            resourceType = (ResourceType)result;
        }
        else
        {
            type = type.ToLower();

            switch (type)
            {
                case "scraps":
                case "metal":
                case "metals":
                case "metalscrap":
                case "metalscraps":
                    resourceType = ResourceType.Scrap;
                    break;

                case "woods":
                case "plank":
                case "planks":
                    resourceType = ResourceType.Wood;
                    break;

                case "fabrics":
                case "cloth":
                case "cloths":
                case "clothing":
                case "cotton":
                case "cottons":
                    resourceType = ResourceType.Fabric;
                    break;

                case "magnets":
                case "magnetite":
                case "magnetites":
                    resourceType = ResourceType.Magnet;
                    break;

                case "techs":
                case "technology":
                case "elec":
                case "electronic":
                case "electronics":
                    resourceType = ResourceType.Tech;
                    break;

                case "crystals":
                case "thermalcrystal":
                case "thermalcrystals":
                case "gem":
                case "gems":
                    resourceType = ResourceType.Crystal;
                    break;

                default:
                    return;
            }
        }

        Instance.AddResource(resourceType, amount);
    }
    #endregion

    #region Item Debug Commands
    [DebugCommand("Will print out a log with the name of every item in the game", DisplayName = "ListAllItems", CloseDebugConsole = false)]
    public static void ListAllItemsDebug()
    {
        string result = "List of all items in the game:\n";

        foreach (Item item in AssetStorage.GetAssetsOfType<Item>())
        {
            if (item == null) return;

            result += item.Name.Replace(" ", "") + "\n";
        }

        Debug.Log(result.Trim());
    }

    [DebugCommand(DisplayName = "AddItem")]
    public static void AddItemDebug(string name) => AddItemDebug(name, 1);

    [DebugCommand(DisplayName = "AddItem")]
    public static void AddItemDebug(string name, int amount)
    {
        Item item = GetItem(name);

        if (item == null) return;

        Instance.AddItem(item, amount);
        Debug.Log("Added " + amount + " " + item.Name + " to the inventory");
    }

    [DebugCommand(DisplayName = "RemoveItem")]
    public static void RemoveItemDebug(string name, int amount = 1)
    {
        Item item = GetItem(name);

        if (item == null) return;

        Instance.RemoveItem(item, amount);
        Debug.Log("Removed " + amount + " " + item.Name + " from the inventory");
    }

    [DebugCommand(DisplayName = "RemoveAllItems")]
    public static void RemoveAllItemsDebug()
    {
        foreach (Item item in AssetStorage.GetAssetsOfType<Item>())
        {
            Instance.RemoveAllItem(item);
        }
        Debug.Log("Removed all items from inventory");
    }

    private static Item GetItem(string name)
    {
        name = name.Trim();

        bool IsMatch(string s) => s.Replace(" ", "").Equals(name, StringComparison.OrdinalIgnoreCase);

        foreach (Item item in AssetStorage.GetAssetsOfType<Item>())
        {
            if (IsMatch(item.Name) || IsMatch(item.name))
            {
                return item;
            }
        }

        return null;
    }

    [DebugCommand("Items that have a ItemUpgrade component is considered an Upgrade", DisplayName = "AddAllUpgrades")]
    public static void AddAllUpgrades()
    {
        foreach (Item item in AssetStorage.GetAssetsOfType<Item>())
        {
            if (item.HasItemComponent<ItemUpgrade>())
            {
                Instance.AddItem(item);
            }
        }
        Debug.Log("Added all upgrade items to inventory");
    }

    [DebugCommand("Items that have a ItemUpgrade component is considered an Upgrade", DisplayName = "RemoveAllUpgrades")]
    public static void RemoveAllUpgrades()
    {
        foreach (Item item in AssetStorage.GetAssetsOfType<Item>())
        {
            if (item.HasItemComponent<ItemUpgrade>())
            {
                Instance.RemoveAllItem(item);
            }
        }
        Debug.Log("Removed all upgrade items from inventory");
    }
    #endregion

    [Serializable]
    public class SerializableItemAndAmount<T>
    {
        public T Item => item;
        public int Count => count;
        [SerializeField] private T item;
        [SerializeField] private int count;
    }
}
