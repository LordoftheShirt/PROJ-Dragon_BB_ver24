using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIBlueprint : MonoBehaviour
{
    public Blueprint Blueprint
    {
        get => _blueprint;
        set
        {
            _blueprint = value;
            nameText.text = _blueprint.Result == null ? _blueprint.name : _blueprint.Result.Name;
            icon.sprite = _blueprint.Result == null ? null : _blueprint.Result.Icon;
            statusText.text = "Available";
        }
    }
    private Blueprint _blueprint;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;

    [Space]
    [SerializeField] private Image bg;
    [SerializeField] private Color selectedColor = Color.white;
    private Color _unselectedColor;

    private CraftingUI _craftingUI;

    private void Awake()
    {
        _craftingUI = GetComponentInParent<CraftingUI>(true);
        _unselectedColor = bg.color;
        SetState(false);
    }

    public void OnClick()
    {
        _craftingUI.SelectedCraftingUIBlueprint = this;
        _craftingUI.SelectedBlueprint = Blueprint;
    }

    public void SetState(bool selected, bool instant = false)
    {
        Color col = selected ? selectedColor : _unselectedColor;

        bg.DOKill();

        if (instant)
        {
            bg.color = col;
            return;
        }

        bg.DOColor(col, 0.1f);
    }
}
