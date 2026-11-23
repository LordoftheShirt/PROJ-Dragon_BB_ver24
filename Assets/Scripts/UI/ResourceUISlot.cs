using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUISlot : MonoBehaviour
{
    public ResourceType Type
    {
        get => _type.HasValue ? _type.Value : default;
        set
        {
            if (_type.HasValue && _type == value) return;

            _type = value;
            icon.sprite = EnumIcons.GetResourceIcon(value);
            UpdateCountText(Inventory.Instance.GetResource(value));
        }
    }
    private ResourceType? _type;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    private void Awake()
    {
        Inventory.OnResourceChanged += OnResourceChanged;
    }

    private void OnDestroy()
    {
        Inventory.OnResourceChanged -= OnResourceChanged;
    }

    private void OnResourceChanged(ResourceType type, int count, int change)
    {
        if (type == Type)
        {
            UpdateCountText(count);
        }
    }

    private void UpdateCountText(int count)
    {
        countText.text = count.ToString();
    }
}
