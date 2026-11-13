using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Custom/Blueprint")]
public class Blueprint : ScriptableObject
{
    public Item Result => result;
    public BlueprintCategory Category => category;
    public Ingredient[] Ingredients => ingredients;

    [SerializeField] private Item result;
    [SerializeField] private BlueprintCategory category;
    [SerializeField] private Ingredient[] ingredients;

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
