using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUICategory : MonoBehaviour
{
    public BlueprintCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            text.text = _category.GetDisplayString();
            icon.sprite = _category.GetIcon();
        }
    }
    private BlueprintCategory _category;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;

    [Space]
    [SerializeField] private Image bg;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = Color.white;

    private CraftingUI _craftingUI;

    private void Awake()
    {
        _craftingUI = GetComponentInParent<CraftingUI>(true);
        SetState(false);
    }

    public void OnClick()
    {
        _craftingUI.ChangeCategory(Category);
    }

    public void SetState(bool selected, bool instant = false)
    {
        Color col = selected ? selectedColor : unselectedColor;
        Color invCol = !selected ? selectedColor : unselectedColor;

        bg.DOKill();
        icon.DOKill();
        text.DOKill();

        if (instant)
        {
            bg.color = col;
            icon.color = invCol;
            text.color = invCol;
            return;
        }

        bg.DOColor(col, 0.1f).SetUpdate(true);
        icon.DOColor(invCol, 0.1f).SetUpdate(true);
        text.DOColor(invCol, 0.1f).SetUpdate(true);
    }
}
