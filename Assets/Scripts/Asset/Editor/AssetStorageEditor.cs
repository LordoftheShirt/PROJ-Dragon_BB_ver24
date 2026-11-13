using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(AssetStorage))]
public class AssetStorageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        base.OnInspectorGUI();
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        if (GUILayout.Button("Update"))
        {
            Update();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Update()
    {
        AssetStorage instance = Resources.Load<AssetStorage>(nameof(AssetStorage));

        using (SerializedObject obj = new SerializedObject(instance))
        {
            SerializedProperty assetsProp = obj.FindProperty("assets");

            assetsProp.ClearArray();

            foreach (Type type in AssetStorage.AssetTypes)
            {
                foreach (string guid in AssetDatabase.FindAssets($"t: {type.Name}"))
                {
                    // Load asset
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    Object asset = AssetDatabase.LoadAssetAtPath(assetPath, type);

                    // Create a new array element and assign it's value to the asset
                    int index = assetsProp.arraySize;
                    assetsProp.InsertArrayElementAtIndex(index);

                    assetsProp.GetArrayElementAtIndex(index).objectReferenceValue = asset;
                }
            }

            obj.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
