using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTab : MonoBehaviour
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

    private CraftingUI _craftingUI;

    private void Awake()
    {
        _craftingUI = GetComponentInParent<CraftingUI>(true);
    }

    public void ChangeTab()
    {
        _craftingUI.ChangeTab(Category);
    }
}
