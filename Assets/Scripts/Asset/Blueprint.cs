using UnityEngine;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Custom/Blueprint")]
public class Blueprint : ScriptableObject
{
    public Item Result => result;
    public int ResultAmount => resultAmount;
    public BlueprintCategory Category => category;
    public Ingredient[] Ingredients => ingredients;
    public Item[] ItemRequirements => itemRequirements;
    public int SortingOrder => sortingOrder;

    [SerializeField] private Item result;
    [SerializeField] private int resultAmount = 1;
    [SerializeField] private BlueprintCategory category;
    [SerializeField] private Ingredient[] ingredients;
    [SerializeField] private Item[] itemRequirements;
    [SerializeField] private int sortingOrder;

    public bool CanCraft()
    {
        Inventory inv = Inventory.Instance;

        if (result != null && !result.UnlimitedStackSize && inv.GetItemAmount(result) >= result.MaxStackSize)
        {
            return false;
        }

        // Fufill every ingredient requirement
        foreach (var ingredient in ingredients)
        {
            int count = inv.GetResource(ingredient.Type);
            if (count < ingredient.Amount)
            {
                return false;
            }
        }

        // Fufill every item requirement
        if (ItemRequirements != null)
        {
            foreach (var item in itemRequirements)
            {
                if (!inv.HasItem(item))
                {
                    return false;
                }
            }
        }

        return true;
    }

    [System.Serializable]
    public class Ingredient
    {
        public ResourceType Type => type;
        public int Amount => amount;
        [SerializeField] private ResourceType type;
        [SerializeField] private int amount;

#if UNITY_EDITOR
        // Custom property drawer for Ingredient class so it looks better in the inspector
        [CustomPropertyDrawer(typeof(Ingredient))]
        public class IngredientPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
                // Draw the ResourceType field
                SerializedProperty typeProperty = property.FindPropertyRelative("type");
                Rect typeRect = new Rect(position.x, position.y, position.width / 2 - 2, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(typeRect, typeProperty);

                // Draw the Amount field
                SerializedProperty amountProperty = property.FindPropertyRelative("amount");
                Rect amountRect = new Rect(position.x + position.width / 2 + 2, position.y, position.width / 2 - 2, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(amountRect, amountProperty);

                EditorGUI.EndProperty();
            }
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Two lines for ResourceType and Amount
            }
        }
#endif
    }
}

public class BlueprintComparer : IComparer<Blueprint>
{
    public int Compare(Blueprint x, Blueprint y)
    {
        int xCategory = x.Category.GetIndex();
        int yCategory = y.Category.GetIndex();

        if (xCategory == yCategory)
        {
            return x.SortingOrder.CompareTo(y.SortingOrder);
        }

        return xCategory.CompareTo(yCategory);
    }
}