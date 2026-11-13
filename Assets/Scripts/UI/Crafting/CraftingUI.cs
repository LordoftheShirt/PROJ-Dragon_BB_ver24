using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private CraftingTab tabTemplate;
    private GameObject[] _tab;

    private Blueprint[] _blueprints;

    private void Start()
    {
        _blueprints = AssetStorage.GetAssetsOfType<Blueprint>();

        // Create categories
        foreach (BlueprintCategory category in System.Enum.GetValues(typeof(BlueprintCategory)))
        {
            CraftingTab newTab = Instantiate(tabTemplate, tabTemplate.transform.parent);
            newTab.Category = category;
        }

        // Hide template
        tabTemplate.gameObject.SetActive(false);
    }

    public void ChangeTab(BlueprintCategory category)
    {
        // Logic to change the crafting tab based on the category
        Debug.Log($"Changing to tab: {category.GetDisplayString()}");
    }
}
