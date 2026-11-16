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

    private CraftingUI _craftingUI;

    private void Awake()
    {
        _craftingUI = GetComponentInParent<CraftingUI>(true);
    }

    public void OnClick()
    {
        _craftingUI.SelectedBlueprint = Blueprint;
    }
}
