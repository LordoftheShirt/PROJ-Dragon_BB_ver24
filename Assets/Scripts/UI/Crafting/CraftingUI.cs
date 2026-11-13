using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private List<string> categories = new();
    [SerializeField] private CraftingTab tabTemplate;

    private Recipe[] _recipes;

    private void Start()
    {
        _recipes = AssetStorage.GetAssetsOfType<Recipe>();

        // Add each missing recipe category
        foreach (Recipe recipe in _recipes)
        {
            if (!categories.Contains(recipe.Category))
            {
                categories.Add(recipe.Category);
            }
        }
    }
}
