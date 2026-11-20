using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public Sprite Icon => icon;
    public Color Color => color;
    public Color BackgroundColor => backgroundColor;

    public string Name => itemName;
    public string Description => description;

    // Override this for a dynamic stack size (ex. one that can be upgraded with another item)
    public virtual int MaxStackSize => maxStackSize;
    public bool UnlimitedStackSize => maxStackSize <= 0;
    public int Durability => durability;

    public bool CanBeBroughtOnExpeditions => canBeBroughtOnExpeditions;
    public EquipmentSlotType EquipmentSlot => equipmentSlot;

    [SerializeField] private Sprite icon;
    [SerializeField] private Color color = Color.clear;
    [SerializeField] private Color backgroundColor = Color.clear;

    [Space]
    [SerializeField] private string itemName;
    [TextArea(1, 5)]
    [SerializeField] private string description;

    [Space]
    [Tooltip("Set to 0 for unlimited stack size")]
    [SerializeField] private int maxStackSize = 0;
    [Tooltip("Measured in days/uses, 0 for unlimited")]
    [SerializeField] private int durability;

    [Space]
    [SerializeField] private bool canBeBroughtOnExpeditions;
    [SerializeField] private EquipmentSlotType equipmentSlot;

    [Space]
    [SerializeField] private string useType;
    public bool CanBeUsed => !string.IsNullOrEmpty(useType);
    private ItemUse _itemUse;

    private void TryCreateItemUse()
    {
        if (_itemUse == null)
        {
            _itemUse = Activator.CreateInstance(Type.GetType(useType)) as ItemUse;
        }
    }

    public void Use()
    {
        if (!CanBeUsed) return;
        TryCreateItemUse();

        _itemUse.Use();
    }

    public void Select()
    {
        if (!CanBeUsed) return;
        TryCreateItemUse();

        _itemUse.Select();
    }

    public void Unselect()
    {
        if (!CanBeUsed) return;
        TryCreateItemUse();

        _itemUse.Unselect();
    }
}
