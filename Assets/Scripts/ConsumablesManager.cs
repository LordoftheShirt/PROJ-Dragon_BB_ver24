using System;
using UnityEngine;

// simply tracks the max and current counts of consumables. Does not communicate out to other scripts.
public class ConsumablesManager : Singleton<ConsumablesManager>
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
    public int GetItemCount(Item item) => _inv.GetItemCount(item);

    private void Start()
    {
        _inv = Inventory.Instance;
    }

    public void AddFlag()
    {
        AddItem(flagItem, OnFlagsChanged);
    }
    public void AddTent()
    {
        AddItem(tentItem, OnTentsChanged);
    }
    public void AddFirekit()
    {
        AddItem(firekitItem, OnFirekitsChanged);
    }
    private void AddItem(Item item, Action<int> action)
    {
        int count = GetItemCount(item);
        if (count < GetMaxStack(item))
        {
            _inv.AddItem(item);

            count++;
            action?.Invoke(count);
        }
    }

    public void UseFlag()
    {
        UseItem(flagItem, OnFlagsChanged);
    }
    public void UseTent()
    {
        UseItem(tentItem, OnTentsChanged);
    }
    public void UseFirekit()
    {
        UseItem(firekitItem, OnFirekitsChanged);
    }
    private void UseItem(Item item, Action<int> action)
    {
        int count = GetItemCount(item);
        if (count > 0)
        {
            _inv.RemoveItem(item);

            count--;
            action?.Invoke(count);
        }
    }
}
