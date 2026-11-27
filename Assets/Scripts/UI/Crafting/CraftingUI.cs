using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftingUI : Singleton<CraftingUI>
{
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;

            if (_isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }
    private bool _isOpen;

    private static readonly BlueprintComparer _blueprintComparer = new();

    [SerializeField] private bool debugMode;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private CraftingUICategory categoryTemplate;
    private CraftingUICategory[] _categories;
    [SerializeField] private CraftingUIBlueprint blueprintTemplate;
    private Dictionary<BlueprintCategory, List<CraftingUIBlueprint>> _blueprintUIMap = new();

    [Space]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private CraftingUIInspector inspector;

    [Space]
    [SerializeField] private ResourceUISlot resourceSlotTemplate;

    [Space]
    [SerializeField] private InputActionReference closeInput;

    private BlueprintCategory _currentCategory;
    private int _categoryAmount;

    public CraftingUIBlueprint SelectedCraftingUIBlueprint
    {
        get => _selectedCraftingUIBlueprint;
        set
        {
            if (_selectedCraftingUIBlueprint != null) _selectedCraftingUIBlueprint.SetState(false);

            _selectedCraftingUIBlueprint = value;

            if (_selectedCraftingUIBlueprint != null) _selectedCraftingUIBlueprint.SetState(true);
        }
    }
    private CraftingUIBlueprint _selectedCraftingUIBlueprint;

    public Blueprint SelectedBlueprint
    {
        get => _selectedBlueprint;
        set
        {
            _selectedBlueprint = value;
            inspector.SelectBlueprint(value);
        }
    }
    private Blueprint _selectedBlueprint;

    private void Start()
    {
        // Create categories
        Array array = Enum.GetValues(typeof(BlueprintCategory));
        _categoryAmount = array.Length;
        _categories = new CraftingUICategory[_categoryAmount];

        for (int i = 0; i < _categoryAmount; i++)
        {
            BlueprintCategory category = (BlueprintCategory)array.GetValue(i);

            CraftingUICategory newCategory = Instantiate(categoryTemplate, categoryTemplate.transform.parent);
            newCategory.Category = category;

            _categories[i] = newCategory;
        }

        // Create resource slots
        ResourceType[] resourceTypes = ResourceTypeUtil.GetAllResourceTypes();
        foreach (ResourceType resourceType in resourceTypes)
        {
            ResourceUISlot slot = Instantiate(resourceSlotTemplate, resourceSlotTemplate.transform.parent);
            slot.Type = resourceType;

            slot.gameObject.SetActive(true);
        }

        // Hide templates
        categoryTemplate.gameObject.SetActive(false);
        blueprintTemplate.gameObject.SetActive(false);
        resourceSlotTemplate.gameObject.SetActive(false);

        mainPanel.SetActive(false);

        closeInput.action.performed += OnCloseInput;
    }

    private void OnDestroy()
    {
        closeInput.action.performed -= OnCloseInput;
    }

    public void ChangeCategory(BlueprintCategory category)
    {
        //Debug.Log($"Changing to category: {category.GetDisplayString()}");

        _currentCategory = category;
        int index = category.GetIndex();

        for (int i = 0; i < _categoryAmount; i++)
        {
            _categories[i].SetState(i == index);
        }

        foreach (var pair in _blueprintUIMap)
        {
            bool enable = pair.Key == category;

            foreach (var blueprint in pair.Value)
            {
                blueprint.gameObject.SetActive(enable);
            }
        }
    }

    public void Open()
    {
        GameCursor.Visible = false;
        Time.timeScale = 0;

        mainPanel.SetActive(true);

        scrollRect.verticalNormalizedPosition = 0;

        foreach (var pair in _blueprintUIMap)
        {
            foreach (var craftingBlueprint in pair.Value)
            {
                Destroy(craftingBlueprint.gameObject);
            }
        }

        _blueprintUIMap.Clear();

        // Create blueprints
        List<Blueprint> blueprints =
            debugMode ? AssetStorage.GetAssetsOfType<Blueprint>().ToList() :
            Inventory.BlueprintsUnlocked;

        blueprints.Sort(_blueprintComparer);

        foreach (Blueprint blueprint in blueprints)
        {
            if (blueprint == null) continue;

            CraftingUIBlueprint newBlueprint = Instantiate(blueprintTemplate, blueprintTemplate.transform.parent);
            newBlueprint.Blueprint = blueprint;

            if (!_blueprintUIMap.ContainsKey(blueprint.Category))
            {
                _blueprintUIMap[blueprint.Category] = new();
            }

            _blueprintUIMap[blueprint.Category].Add(newBlueprint);
        }

        inspector.SelectBlueprint(null, true);
        ChangeCategory(_currentCategory);
    }

    public void Close()
    {
        GameCursor.Visible = true;
        Time.timeScale = 1;

        InputMapManager.Instance.EnablePlayerMap();
        mainPanel.SetActive(false);

        InputMapManager.Instance.EnablePlayerMap();
    }

    private void OnCloseInput(InputAction.CallbackContext ctx)
    {
        if (!IsOpen) return;

        Close();
    }
}
