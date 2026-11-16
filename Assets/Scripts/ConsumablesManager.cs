using System;
using UnityEngine;

// simply tracks the max and current counts of consumables. Does not communicate out to other scripts.
[LoadOnInit]
public class ConsumablesManager : SingletonPersistent<ConsumablesManager>
{
    /*
    [Header("Flags")]
    [SerializeField] private int maxFlags = 5;
    [SerializeField] private int currentFlags = 3;

    [Header("Tents")]
    [SerializeField] private int maxTents = 3;
    [SerializeField] private int currentTents = 2;

    [Header("Firekits")]
    [SerializeField] private int maxFirekits = 3;
    [SerializeField] private int currentFirekits = 2;
    */

    [Header("Items")]
    [SerializeField] private Item flagItem;
    [SerializeField] private Item tentItem;
    [SerializeField] private Item firekitItem;

    private Inventory _inv;

    public event Action<int> OnFlagsChanged;
    public event Action<int> OnTentsChanged;
    public event Action<int> OnFirekitsChanged;

    public int GetMaxFlags() => GetMaxStack(flagItem);
    public int GetMaxTents() => GetMaxStack(tentItem);
    public int GetMaxFirekits() => GetMaxStack(firekitItem);
    public int GetMaxStack(Item item) => (item == null || item.UnlimitedStackSize) ? int.MaxValue : item.MaxStackSize;

    public int GetCurrentFlags() => GetItemCount(flagItem);
    public int GetCurrentTents() => GetItemCount(tentItem);
    public int GetCurrentFirekits() => GetItemCount(firekitItem);
    public int GetItemCount(Item item) => _inv.GetItem(item);

    private void Start()
    {
        _inv = Inventory.Instance;

        Inventory.OnItemChanged += OnItemChanged;
    }

    private void OnDestroy()
    {
        Inventory.OnItemChanged -= OnItemChanged;
    }

    private void OnItemChanged(Item item, int count, int change)
    {
        if (item == flagItem) OnFlagsChanged?.Invoke(count);
        else if (item == tentItem) OnTentsChanged?.Invoke(count);
        else if (item == firekitItem) OnFirekitsChanged?.Invoke(count);
    }

    public bool AddFlag()
    {
        return AddItem(flagItem);
    }
    public bool AddTent()
    {
        return AddItem(tentItem);
    }
    public bool AddFirekit()
    {
        return AddItem(firekitItem);
    }
    private bool AddItem(Item item)
    {
        if (GetItemCount(item) < GetMaxStack(item))
        {
            return _inv.AddItem(item);
        }

        return false;
    }

    public bool UseFlag()
    {
        return UseItem(flagItem);
    }
    public bool UseTent()
    {
        return UseItem(tentItem);
    }
    public bool UseFirekit()
    {
        return UseItem(firekitItem);
    }
    private bool UseItem(Item item)
    {
        return _inv.RemoveItem(item);
    }
}
