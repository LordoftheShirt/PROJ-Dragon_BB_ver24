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
    }

    public void OnClick()
    {
        _craftingUI.ChangeCategory(Category);
    }

    public void SetState(bool selected)
    {
        bg.color = selected ? selectedColor : unselectedColor;
    }
}
