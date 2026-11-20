using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    private const string USE_TYPE_PROP = "useType";

    private static Type[] _itemUseTypes = null;

    [InitializeOnLoadMethod]
    private static void Init()
    {
        List<Type> list = new();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var assemblyType in assembly.GetTypes())
            {
                if (typeof(ItemUse).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract)
                {
                    list.Add(assemblyType);
                }
            }
        }

        _itemUseTypes = list.ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            string propPath = iterator.propertyPath;

            if (propPath == USE_TYPE_PROP)
            {
                EditorGUILayout.Space();
                Rect rect = EditorGUILayout.GetControlRect();
                rect = EditorGUI.PrefixLabel(rect, new("Use Type"));

                Type type;
                if (string.IsNullOrEmpty(iterator.stringValue))
                {
                    type = null;
                }
                else
                {
                    type = Type.GetType(iterator.stringValue);
                }

                if (EditorGUI.DropdownButton(rect, new(type == null ? "<NONE>" : type.Name), FocusType.Passive))
                {
                    GenericMenu menu = new();

                    void Set(string s)
                    {
                        serializedObject.UpdateIfRequiredOrScript();
                        serializedObject.FindProperty(USE_TYPE_PROP).stringValue = s;
                        serializedObject.ApplyModifiedProperties();
                    }

                    menu.AddItem(new("<NONE>"), type == null, () =>
                    {
                        Set("");
                    });

                    menu.AddSeparator("");

                    foreach (var itemUseType in _itemUseTypes)
                    {
                        Type cache = itemUseType;

                        menu.AddItem(new(cache.Name), type == cache, () =>
                        {
                            Set(cache.AssemblyQualifiedName);
                        });
                    }

                    menu.DropDown(rect);
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(propPath == "m_Script"))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }

            enterChildren = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
