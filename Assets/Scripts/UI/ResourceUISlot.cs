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
            icon.sprite = EnumStyle.GetResourceIcon(value);
            UpdateCountText(Inventory.Instance.GetResource(value));

            Color col = EnumStyle.GetResourceColor(value);
            Color bgCol = EnumStyle.GetResourceBackgroundColor(value);

            icon.color = col;
            countText.color = col;

            _material.SetColor(SdfShaderProperties.OUTLINE_COLOR, col);
            _material.SetColor(SdfShaderProperties.COLOR, bgCol);
        }
    }
    private ResourceType? _type;

    [SerializeField] private Graphic sdfGraphic;
    private Material _material;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    private void Awake()
    {
        sdfGraphic.material = new Material(sdfGraphic.material);
        _material = sdfGraphic.material;

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
