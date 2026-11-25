using DG.Tweening;
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
            TryMakeMaterials();

            _type = value;
            icon.sprite = EnumStyle.GetResourceIcon(value);
            int count = Inventory.Instance.GetResource(value);
            UpdateCountText(count);

            if (nameText != null) nameText.text = Type.GetDisplayString(true);

            UpdateColor(count, true);
        }
    }
    private ResourceType? _type;

    [SerializeField] private Graphic sdfGraphic;
    private Material _material;
    private bool _madeMaterials;
    [SerializeField] private Graphic[] colorGraphics;
    [SerializeField] private Graphic[] backgroundColorGraphics;

    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;

    private bool _isDisabledColor;

    private void OnEnable()
    {
        TryMakeMaterials();

        Inventory.OnResourceChanged += OnResourceChanged;

        if (_type.HasValue)
        {
            UpdateCountText(Inventory.Instance.GetResource(_type.Value), true);
        }
    }

    private void TryMakeMaterials()
    {
        if (_madeMaterials) return;
        _madeMaterials = true;

        if (sdfGraphic != null)
        {
            sdfGraphic.material = new Material(sdfGraphic.material);
            _material = sdfGraphic.material;
        }
    }

    private void OnDisable()
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

    private void UpdateCountText(int count, bool forced = false)
    {
        if (countText != null) countText.text = count.ToString();

        UpdateColor(count, forced);
    }

    private void UpdateColor(int count, bool forced = false)
    {
        bool disabledColor = count <= 0;

        if (!forced && disabledColor == _isDisabledColor) return;
        _isDisabledColor = disabledColor;

        Color col = disabledColor ? EnumStyle.DisabledColor : EnumStyle.GetResourceColor(Type);
        Color bgCol = disabledColor ? EnumStyle.DisabledBackgroundColor : EnumStyle.GetResourceBackgroundColor(Type);

        icon.color = col;
        countText.color = col;

        if (_material != null)
        {
            _material.SetColor(SdfShaderProperties.OUTLINE_COLOR, col);
            _material.SetColor(SdfShaderProperties.COLOR, bgCol);
        }

        if (colorGraphics != null)
        {
            foreach (var graphic in colorGraphics)
            {
                graphic.color = col;
            }
        }

        if (backgroundColorGraphics != null)
        {
            foreach (var graphic in backgroundColorGraphics)
            {
                graphic.color = bgCol;
            }
        }
    }
}
