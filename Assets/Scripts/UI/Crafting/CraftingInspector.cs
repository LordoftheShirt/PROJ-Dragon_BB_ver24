using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingInspector : MonoBehaviour
{
    [SerializeField] private GameObject noBlueprintView;
    [SerializeField] private GameObject normalView;

    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    [Space]
    [SerializeField] private Button craftButton;

    private Blueprint _currentBlueprint;

    private CraftingUI _craftingUI;

    private void Awake()
    {
        _craftingUI = GetComponentInParent<CraftingUI>(true);
    }

    public void SelectBlueprint(Blueprint blueprint)
    {
        _currentBlueprint = blueprint;

        if (blueprint == null)
        {
            noBlueprintView.SetActive(true);
            normalView.SetActive(false);
            return;
        }

        noBlueprintView.SetActive(false);
        normalView.SetActive(true);

        Item item = blueprint.Result;
        icon.sprite = item == null ? null : item.Icon;
        nameText.text = item == null ? blueprint.name : item.Name;
        descriptionText.text = item == null ? "<MISSING DESCRIPTION>" : item.Description;

        craftButton.interactable = blueprint.CanCraft();
    }

    public void Craft()
    {
        if (_currentBlueprint == null || !craftButton.interactable) return;

        _craftingUI.CraftBlueprint(_currentBlueprint);
    }
}
