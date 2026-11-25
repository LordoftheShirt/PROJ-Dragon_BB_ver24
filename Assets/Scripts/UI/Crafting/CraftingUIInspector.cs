using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIInspector : MonoBehaviour
{
    [SerializeField] private GameObject noBlueprintView;
    [SerializeField] private GameObject normalView;

    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    //[SerializeField] private TMP_Text requirementsText;
    [SerializeField] private TMP_Text amountText;

    [Space]
    [SerializeField] private CraftingUIRequirement requirementTemplate;
    [SerializeField] private Button craftButton;

    private List<CraftingUIRequirement> _requirements = new();
    private Blueprint _currentBlueprint;
    private CraftingUI _craftingUI;

    private void Awake()
    {
        requirementTemplate.gameObject.SetActive(false);
        _craftingUI = GetComponentInParent<CraftingUI>(true);
    }

    public void SelectBlueprint(Blueprint blueprint, bool forced = false)
    {
        if (!forced && blueprint == _currentBlueprint) return;

        foreach (var requirement in _requirements)
        {
            Destroy(requirement.gameObject);
        }
        _requirements.Clear();

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

        CraftingUIRequirement CreateRequirement()
        {
            var requirement = Instantiate(requirementTemplate, requirementTemplate.transform.parent);
            requirement.gameObject.SetActive(true);

            _requirements.Add(requirement);

            return requirement;
        }

        foreach (var pair in blueprint.Ingredients)
        {
            var requirement = CreateRequirement();

            ResourceType type = pair.Type;
            requirement.Name = type.GetDisplayString(true);
            requirement.Type = type;
            requirement.RequiredAmount = pair.Amount;
        }

        if (blueprint.ItemRequirements != null)
        {
            foreach (var itemRequirement in blueprint.ItemRequirements)
            {
                var requirement = CreateRequirement();

                requirement.Name = itemRequirement.Name;
                requirement.Item = itemRequirement;
                requirement.RequiredAmount = 1;
            }
        }

        UpdateInteractable();
    }

    public void Craft()
    {
        if (_currentBlueprint == null || !craftButton.interactable) return;

        //Debug.Log("CRAFT!");

        Inventory inv = Inventory.Instance;

        foreach (var pair in _currentBlueprint.Ingredients)
        {
            inv.RemoveResource(pair.Type, pair.Amount);
        }

        inv.AddItem(_currentBlueprint.Result, _currentBlueprint.ResultAmount);

        UpdateInteractable();
    }

    private void UpdateInteractable()
    {
        if (_currentBlueprint == null) return;

        Inventory inv = Inventory.Instance;

        // THIS IS A PLACEHOLDER WAY TO DISPLAY THE REQUIREMENTS FOR NOW
        /*
        string text = "Requirements:\n";

        foreach (var pair in _currentBlueprint.Ingredients)
        {
            int amount = inv.GetResource(pair.Type);
            text += $"{pair.Type.GetDisplayString()} <color={(amount >= pair.Amount ? "green" : "red")}>{amount}/{pair.Amount}</color>\n";
        }

        if (_currentBlueprint.ItemRequirements != null)
        {
            foreach (var item in _currentBlueprint.ItemRequirements)
            {
                bool hasItem = inv.HasItem(item);
                text += $"{item.Name} <color={(hasItem ? "green>1/1" : "red>0/1")}</color>\n";
            }
        }

        requirementsText.text = text;
        */

        foreach (var requirement in _requirements)
        {
            requirement.UpdateRequirement();
        }

        Item result = _currentBlueprint.Result;
        amountText.text = result == null || result.UnlimitedStackSize ? "" : $"You have:\n{inv.GetItemAmount(result)}/{result.MaxStackSize}";

        craftButton.interactable = _currentBlueprint.CanCraft();
    }
}
