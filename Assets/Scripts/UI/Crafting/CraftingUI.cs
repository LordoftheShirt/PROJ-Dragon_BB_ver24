using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

#if UNITY_EDITOR
    [SerializeField] private bool debugMode;
#endif
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private CraftingCategory categoryTemplate;
    private CraftingCategory[] _categories;
    [SerializeField] private CraftingBlueprint blueprintTemplate;
    private Dictionary<BlueprintCategory, List<CraftingBlueprint>> _blueprintUIMap = new();

    [Space]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private CraftingInspector inspector;

    private BlueprintCategory _currentCategory;
    private int _categoryAmount;

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
        _categories = new CraftingCategory[_categoryAmount];

        for (int i = 0; i < _categoryAmount; i++)
        {
            BlueprintCategory category = (BlueprintCategory)array.GetValue(i);

            CraftingCategory newCategory = Instantiate(categoryTemplate, categoryTemplate.transform.parent);
            newCategory.Category = category;

            _categories[i] = newCategory;
        }

        // Hide templates
        categoryTemplate.gameObject.SetActive(false);
        blueprintTemplate.gameObject.SetActive(false);

        mainPanel.SetActive(false);
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
#if UNITY_EDITOR
            debugMode ? AssetStorage.GetAssetsOfType<Blueprint>().ToList() :
#endif
            Inventory.BlueprintsUnlocked;

        blueprints.Sort(_blueprintComparer);

        foreach (Blueprint blueprint in blueprints)
        {
            if (blueprint == null) continue;

            CraftingBlueprint newBlueprint = Instantiate(blueprintTemplate, blueprintTemplate.transform.parent);
            newBlueprint.Blueprint = blueprint;

            if (!_blueprintUIMap.ContainsKey(blueprint.Category))
            {
                _blueprintUIMap[blueprint.Category] = new();
            }

            _blueprintUIMap[blueprint.Category].Add(newBlueprint);
        }

        inspector.SelectBlueprint(null);
        ChangeCategory(_currentCategory);
    }

    public void Close()
    {
        InputMapManager.Instance.EnablePlayerMap();
        mainPanel.SetActive(false);

        InputMapManager.Instance.EnablePlayerMap();
    }
}
