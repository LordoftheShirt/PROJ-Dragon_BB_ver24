using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public const int SPACING = 8;

    private static readonly Regex _componentNameRegex = new Regex(@"^item[s]?(.+)", RegexOptions.IgnoreCase);
    private static readonly Regex _componentNumberIndexRegex = new Regex(@"(.*) [0-9]+");
    private static readonly string _componentResetRegexFormat = @"\""{0}\"":[^,]*,";

    private static readonly string[] _removedResetJsonValues = new string[]
    {
        "m_Enabled",
        "m_EditorHideFlags",
        "m_Name",
        "m_EditorClassIdentifier",
        "expandedInEditor",
        "orderInEditor",
        "item",
        "disableOnStart",
    };

    private ItemComponentSearchWindowProvider _searchWindowProvider;

    private SerializedProperty _itemComponentsProperty;
    private bool _shouldCacheItemComponents = false;

    private Dictionary<Object, Editor> _editors = new();

    private Object _pendingRenameObj = null;
    private Object _currentRenameObj = null;

    private string _currentDragArea;

    private void OnEnable()
    {
        _itemComponentsProperty = serializedObject.FindProperty("itemComponents");

        _searchWindowProvider = CreateInstance<ItemComponentSearchWindowProvider>();

        _searchWindowProvider.OnSelectType = (type) =>
        {
            string name = type.Name;

            ScriptableObject newComponent = CreateInstance(type);

            Match match = _componentNameRegex.Match(name);

            if (match.Success)
            {
                newComponent.name = match.Result("$1").Trim();
            }
            else
            {
                newComponent.name = name;
            }

            name = newComponent.name;

            // Ensure the name is unique
            int arraySize = _itemComponentsProperty.arraySize;

            bool itemWithSameNameExists = false;
            int numberOfTries = 1;

            do
            {
                if (itemWithSameNameExists)
                {
                    Match numberIndexMatch = _componentNumberIndexRegex.Match(name);

                    name = (numberIndexMatch.Success ? numberIndexMatch.Result("$1").Trim() : name) + " " + numberOfTries;
                    newComponent.name = name;
                    numberOfTries++;
                }

                bool foundMatch = false;

                for (int i = 0; i < arraySize; i++)
                {
                    Object obj = _itemComponentsProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                    if (obj == null)
                    {
                        continue;
                    }

                    if (obj.name == name)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                itemWithSameNameExists = foundMatch;
            }
            while (itemWithSameNameExists);

            AssetDatabase.AddObjectToAsset(newComponent, target);

            using (SerializedObject serializedObject = new SerializedObject(newComponent))
            {
                serializedObject.FindProperty("orderInEditor").intValue = _itemComponentsProperty.arraySize;

                serializedObject.ApplyModifiedProperties();
            }

            string undoName = $"Added " + name + " to " + target.name;
            Undo.RegisterCreatedObjectUndo(newComponent, undoName);

            Undo.SetCurrentGroupName(undoName);

            SaveAsset();

            _shouldCacheItemComponents = true;
        };

        CacheItemComponents();

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;

        DestroyImmediate(_searchWindowProvider);
    }

    private void OnUndoRedo()
    {
        SaveAsset();

        _editors.Clear();
    }

    private void SaveAsset()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssetIfDirty(target);

        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target), ImportAssetOptions.ForceUpdate);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawItemProperties();

        if (_shouldCacheItemComponents)
        {
            CacheItemComponents();

            _shouldCacheItemComponents = false;
        }

        DrawLine();

        bool shouldSaveAsset = false;
        Rect? renameRect = null;

        int arraySize = _itemComponentsProperty.arraySize;
        for (int i = 0; i < arraySize; i++)
        {
            SerializedProperty arrayElement = _itemComponentsProperty.GetArrayElementAtIndex(i);
            Object arrayObj = arrayElement.objectReferenceValue;

            if (arrayObj == null)
            {
                continue;
            }

            EditorGUILayout.Space(-EditorGUIUtility.standardVerticalSpacing);

            _editors.TryGetValue(arrayObj, out Editor editor);

            CreateCachedEditor(arrayObj, null, ref editor);

            _editors[arrayObj] = editor;

            SerializedProperty expandedProp = editor.serializedObject.FindProperty("expandedInEditor");

            GUIContent foldoutContent = EditorGUIUtility.ObjectContent(arrayObj, arrayObj.GetType());
            string foloutText = foldoutContent.text;
            foldoutContent.text = null;

            Rect rect = EditorGUILayout.GetControlRect();

            Rect enabledRect = rect;

            enabledRect.x += 22;
            enabledRect.width = 20;

            SerializedProperty disabledProp = editor.serializedObject.FindProperty("disableOnStart");
            string toggleName = $"CARD_EDITOR:{target.name}_disableOnStart_toggle_{i}";

            Event evt = Event.current;

            if (enabledRect.Contains(evt.mousePosition) && evt.type == EventType.MouseDown && evt.button == 0)
            {
                disabledProp.boolValue = !disabledProp.boolValue;

                evt.Use();

                GUI.FocusControl(toggleName);
            }

            // Drag and drop
            string dragAreaName = $"CARD_EDITOR:{target.name}_drag_area_{i}";

            if (string.IsNullOrEmpty(_currentDragArea))
            {
                if (rect.Contains(evt.mousePosition) && evt.type == EventType.MouseDown && evt.button == 0)
                {
                    _currentDragArea = dragAreaName;
                    evt.Use();
                }
            }
            else if (evt.button == 0 && _currentDragArea == dragAreaName)
            {
                switch (evt.type)
                {
                    case EventType.MouseDrag:
                        if (!rect.Contains(evt.mousePosition))
                        {
                            break;
                        }

                        DragAndDrop.PrepareStartDrag();

                        DragAndDrop.objectReferences = new Object[1] { arrayObj };
                        DragAndDrop.StartDrag("Dragging " + arrayObj);

                        _currentDragArea = null;

                        evt.Use();
                        break;

                    case EventType.MouseUp:
                        expandedProp.boolValue = !expandedProp.boolValue;

                        _currentDragArea = null;

                        evt.Use();
                        break;
                }
            }

            EditorGUI.BeginFoldoutHeaderGroup(rect, expandedProp.boolValue, foldoutContent, null, (rect) =>
            {
                OpenItemComponentContextMenu(arrayObj, rect);
            });
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUI.SetNextControlName(toggleName);
            EditorGUI.Toggle(enabledRect, !disabledProp.boolValue);

            rect.x += 39;
            rect.width -= 39;

            bool shouldRename = _pendingRenameObj == arrayObj || _currentRenameObj == arrayObj;

            if (!shouldRename)
            {
                EditorGUI.LabelField(rect, foloutText, EditorStyles.boldLabel);
            }
            else
            {
                renameRect = rect;

                _currentRenameObj = arrayObj;
            }

            editor.serializedObject.ApplyModifiedProperties();

            if (rect.Contains(evt.mousePosition))
            {
                switch (evt.type)
                {
                    case EventType.ContextClick:
                        OpenItemComponentContextMenu(arrayObj, null);
                        evt.Use();
                        break;
                }
            }

            if (expandedProp.boolValue)
            {
                DrawLine(false, 0.35f);

                if (editor is ItemComponentEditor)
                {
                    (editor as ItemComponentEditor).ItemEditorInspector();
                }
                else
                {
                    editor.OnInspectorGUI();
                }

                DrawLine();
            }
            else
            {
                DrawLine(false);
            }
        }

        if (renameRect.HasValue)
        {
            GUI.SetNextControlName("RenameField");
            string oldName = _currentRenameObj.name;
            Undo.RecordObject(_currentRenameObj, "Rename " + oldName);
            _currentRenameObj.name = EditorGUI.DelayedTextField(renameRect.Value, oldName);

            Event evt = Event.current;

            if (_pendingRenameObj != null)
            {
                _pendingRenameObj = null;

                EditorGUI.FocusTextInControl("RenameField");
            }
            else if ((evt.type != EventType.Repaint && evt.type != EventType.Layout && GUI.GetNameOfFocusedControl() != "RenameField") || oldName != _currentRenameObj.name)
            {
                SaveAsset();
                //Repaint();

                _currentRenameObj = null;
            }
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect buttonRect = GUILayoutUtility.GetRect(240, 25);

        if (GUI.Button(buttonRect, "Add Item Component"))
        {
            Vector2 guiPoint = new Vector2(buttonRect.center.x, buttonRect.max.y + buttonRect.height / 2 + 10);
            Vector2 mousePoint = GUIUtility.GUIToScreenPoint(guiPoint);

            SearchWindow.Open(new SearchWindowContext(mousePoint), _searchWindowProvider);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        if (shouldSaveAsset)
        {
            SaveAsset();
        }
    }

    private void DrawItemProperties()
    {
        SerializedProperty iterator = serializedObject.GetIterator();

        bool enterChildren = true;
        Rect rect;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            switch (iterator.propertyPath)
            {
                case "m_Script":
                    continue;

                case "description":
                    EditorGUILayout.LabelField(iterator.displayName);

                    GUIStyle style = new GUIStyle(EditorStyles.textArea);
                    style.clipping = TextClipping.Overflow;

                    iterator.stringValue = EditorGUILayout.TextArea(iterator.stringValue, style, GUILayout.Height(60));
                    continue;

                case "icon":
                    rect = EditorGUILayout.GetControlRect(GUILayout.Height(64));

                    GUIContent label = EditorGUI.BeginProperty(rect, new GUIContent(iterator.displayName), iterator);
                    iterator.objectReferenceValue = EditorGUI.ObjectField(rect, label, iterator.objectReferenceValue, typeof(Sprite), false);
                    EditorGUI.EndProperty();
                    continue;
            }

            EditorGUILayout.PropertyField(iterator, true);
        }
    }

    private void DrawLine(bool doSpacing = true, float alpha = 1)
    {
        if (doSpacing)
        {
            EditorGUILayout.Space();
        }

        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(1));

        rect.x -= 30;
        rect.width += 60;

        EditorGUI.DrawRect(rect, new Color(0, 0, 0, alpha));
    }

    private void OpenItemComponentContextMenu(Object itemComponent, Rect? rect)
    {
        GenericMenu menu = new GenericMenu();

        int index = -1;
        int arraySize = _itemComponentsProperty.arraySize;

        Object GetArrayObj(int index) => _itemComponentsProperty.GetArrayElementAtIndex(index).objectReferenceValue;

        for (int i = 0; i < arraySize; i++)
        {
            if (GetArrayObj(i) != itemComponent)
            {
                continue;
            }

            index = i;
            break;
        }

        menu.AddItem(new GUIContent("Rename Component"), false, () =>
        {
            _pendingRenameObj = itemComponent;
        });

        menu.AddItem(new GUIContent("Delete Component"), false, () =>
        {
            _editors.Remove(itemComponent);

            string name = itemComponent.name;

            Undo.DestroyObjectImmediate(itemComponent);

            //AssetDatabase.RemoveObjectFromAsset(target);
            Undo.SetCurrentGroupName("Deleted " + name + " from " + target.name);

            SaveAsset();

            _shouldCacheItemComponents = true;
        });

        menu.AddSeparator("");

        // Swapping orders
        void SwapItemOrder(int indexA, int indexB)
        {
            Object objA = GetArrayObj(indexA);
            Object objB = GetArrayObj(indexB);

            using (SerializedObject serializedObjA = new SerializedObject(objA))
            {
                using (SerializedObject serializedObjB = new SerializedObject(objB))
                {
                    SerializedProperty propA = serializedObjA.FindProperty("orderInEditor");
                    SerializedProperty propB = serializedObjB.FindProperty("orderInEditor");

                    int temp = propA.intValue;
                    propA.intValue = propB.intValue;
                    propB.intValue = temp;

                    serializedObjA.ApplyModifiedProperties();
                    serializedObjB.ApplyModifiedProperties();
                }
            }

            _shouldCacheItemComponents = true;
        }

        menu.AddItem(new GUIContent("Move up"), false, index <= 0 ? null : () =>
        {
            SwapItemOrder(index, index - 1);
        });

        menu.AddItem(new GUIContent("Move down"), false, index < 0 || index >= arraySize - 1 ? null : () =>
        {
            SwapItemOrder(index, index + 1);
        });

        menu.AddSeparator("");

        /*
        menu.AddItem(new GUIContent("Select in inspector"), false, () =>
        {
            Selection.activeObject = itemComponent;
            EditorGUIUtility.PingObject(itemComponent);
        });
        */

        menu.AddItem(new GUIContent("Edit Script"), false, () =>
        {
            using (SerializedObject obj = new SerializedObject(itemComponent))
            {
                EditorUtility.OpenWithDefaultApp(AssetDatabase.GetAssetPath(obj.FindProperty("m_Script").objectReferenceValue));
            }
        });

        menu.AddItem(new GUIContent("Reset"), false, () =>
        {
            Undo.RecordObject(itemComponent, "Reset " + itemComponent.name);

            Object defaultInstance = CreateInstance(itemComponent.GetType());

            string json = EditorJsonUtility.ToJson(defaultInstance);

            foreach (string removedJsonValue in _removedResetJsonValues)
            {
                json = Regex.Replace(json, string.Format(_componentResetRegexFormat, removedJsonValue), "");
            }

            EditorJsonUtility.FromJsonOverwrite(json, itemComponent);
        });

        if (rect.HasValue)
        {
            menu.DropDown(rect.Value);
        }
        else
        {
            menu.ShowAsContext();
        }
    }

    private void CacheItemComponents()
    {
        _itemComponentsProperty.ClearArray();

        List<(int, Object)> sortedList = new List<(int, Object)>();

        int index = 0;
        foreach (Object obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(target)))
        {
            if (obj == null)
            {
                continue;
            }

            if (!typeof(ItemComponent).IsAssignableFrom(obj.GetType()))
            {
                continue;
            }

            using (SerializedObject serializedObject = new SerializedObject(obj))
            {
                sortedList.Add((serializedObject.FindProperty("orderInEditor").intValue, obj));
            }
        }

        sortedList.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        foreach (var pair in sortedList)
        {
            using (SerializedObject serializedObject = new SerializedObject(pair.Item2))
            {
                serializedObject.FindProperty("orderInEditor").intValue = index;
                serializedObject.ApplyModifiedProperties();
            }

            _itemComponentsProperty.InsertArrayElementAtIndex(index);
            _itemComponentsProperty.GetArrayElementAtIndex(index).objectReferenceValue = pair.Item2;

            index++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
