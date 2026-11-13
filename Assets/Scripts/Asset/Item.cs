using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public Sprite Icon => icon;
    public string Name => itemName;
    public string Description => description;
    // Override this for a dynamic stack size (ex. one that can be upgraded with another item)
    public virtual int MaxStackSize => maxStackSize;
    public bool UnlimitedStackSize => maxStackSize <= 0;

    [SerializeField] private Sprite icon;
    [SerializeField] private string itemName;
    [TextArea(1, 5)]
    [SerializeField] private string description;
    [Tooltip("Set to 0 for unlimited stack size")]
    [SerializeField] private int maxStackSize = 0;
}
