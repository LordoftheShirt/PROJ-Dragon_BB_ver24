using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public Sprite Icon => icon;
    public string Name => itemName;
    public string Description => description;

    [SerializeField] private Sprite icon;
    [SerializeField] private string itemName;
    [TextArea(1, 5)]
    [SerializeField] private string description;
}
