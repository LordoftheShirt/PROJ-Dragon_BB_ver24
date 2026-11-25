using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom editor script for <see cref="ItemComponent"/>.
/// </summary>
[CustomEditor(typeof(ItemComponent), true)]
public class ItemComponentEditor : Editor
{
    private SerializedProperty _itemProp;

    private void OnEnable()
    {
        if (target == null || serializedObject == null)
        {
            return;
        }

        _itemProp = serializedObject.FindProperty("item");
    }

    public override void OnInspectorGUI()
    {
        ItemEditorInspector();
    }

    public virtual void ItemEditorInspector()
    {
        if (target == null)
        {
            return;
        }

        bool isSubAsset = AssetDatabase.IsSubAsset(target);

        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty iterator = serializedObject.GetIterator();

        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            switch (iterator.propertyPath)
            {
                case "m_Script":
                    continue;
            }

            EditorGUILayout.PropertyField(iterator, true);

            enterChildren = false;
        }

        Object parentAsset = null;
        if (isSubAsset)
        {
            parentAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(target));
            _itemProp.objectReferenceValue = parentAsset;
        }

        serializedObject.ApplyModifiedProperties();
    }
}