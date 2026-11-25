using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public Sprite Icon => icon;
    public Color Color => color;
    public Color BackgroundColor => backgroundColor;

    public string Name => itemName;
    public string Description => description;

    // Override this for a dynamic stack size (ex. one that can be upgraded with another item)
    public virtual int MaxStackSize => maxStackSize;
    public bool UnlimitedStackSize => maxStackSize <= 0;

    [SerializeField] private Sprite icon;
    [SerializeField] private Color color = Color.clear;
    [SerializeField] private Color backgroundColor = Color.clear;

    [Space]
    [SerializeField] private string itemName;
    [TextArea(1, 5)]
    [SerializeField] private string description;

    [Space]
    [Tooltip("Set to 0 for unlimited stack size")]
    [SerializeField] private int maxStackSize = 0;
    /*
    [Tooltip("Measured in days/uses, 0 for unlimited")]
    [SerializeField] private int durability;
    */


    [HideInInspector]
    [SerializeField] private ItemComponent[] itemComponents;
    private Dictionary<Type, ItemComponent[]> _itemComponentTypeDictionary = new();
    private Dictionary<string, ItemComponent> _itemComponentNameDictionary = new();

    [NonSerialized]
    private bool _cachedItemComponents = false;

    public void Use()
    {
        foreach (var component in itemComponents)
        {
            component.Use();
        }
    }

    public void Select()
    {
        foreach (var component in itemComponents)
        {
            component.Select();
        }
    }

    public void Unselect()
    {
        foreach (var component in itemComponents)
        {
            component.Unselect();
        }
    }

    #region GetItemComponent Methods
    private static readonly MethodInfo _createListMethod = typeof(Item).GetMethod(nameof(CreateListMethod), BindingFlags.Static | BindingFlags.NonPublic);
    private static List<T> CreateListMethod<T>() => new();

    private static readonly MethodInfo _createArrayMethod = typeof(Item).GetMethod(nameof(CreateArrayMethod), BindingFlags.Static | BindingFlags.NonPublic);
    private static ItemComponent[] CreateArrayMethod<T>(List<T> list) => list.ToArray() as ItemComponent[];

    private void TryCacheItemComponents()
    {
        if (_cachedItemComponents)
        {
            return;
        }

        _cachedItemComponents = true;

        _itemComponentTypeDictionary.Clear();
        _itemComponentNameDictionary.Clear();

        Dictionary<Type, IList> temp = new();

        foreach (ItemComponent itemComponent in itemComponents)
        {
            void AddType(Type type)
            {
                if (!temp.ContainsKey(type))
                {
                    temp.Add(type, _createListMethod.MakeGenericMethod(type).Invoke(this, null) as IList);
                }

                temp[type].Add(itemComponent);
            }

            void AddInterfaces(Type type)
            {
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    AddType(interfaceType);
                }
            }

            Type type = itemComponent.GetType();

            AddType(type);
            AddInterfaces(type);

            while (type.BaseType != null && type.BaseType != typeof(ScriptableObject))
            {
                type = type.BaseType;

                AddType(type);
                AddInterfaces(type);
            }

            string name = itemComponent.name.Trim().ToLower();

            if (!_itemComponentNameDictionary.ContainsKey(name))
            {
                _itemComponentNameDictionary.Add(name, itemComponent);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"The Item: \"{this.name}\" has more than one ItemComponent named \"{name}\"! Please rename them in the inspector.", this);
            }
#endif
        }

        foreach (var pair in temp)
        {
            Type type = pair.Key;
            ItemComponent[] array = _createArrayMethod.MakeGenericMethod(type).Invoke(null, new object[] { pair.Value }) as ItemComponent[];

            _itemComponentTypeDictionary.Add(type, array);
        }

        foreach (ItemComponent itemComponent in itemComponents)
        {
            itemComponent.InternalInitialize();
        }
    }

    public T GetItemComponent<T>() where T : ItemComponent
    {
        return GetItemComponent(typeof(T)) as T;
    }

    public ItemComponent GetItemComponent(Type type)
    {
        ItemComponent[] itemComponents = GetItemComponents(type);

        if (itemComponents == null)
        {
            return null;
        }

        return itemComponents[0];
    }

    public ItemComponent GetItemComponent(string name, bool formatName = false)
    {
        if (TryGetItemComponent(name, out ItemComponent result, formatName))
        {
            return result;
        }

        return null;
    }

    public T[] GetItemComponents<T>() where T : ItemComponent
    {
        ItemComponent[] itemComponents = GetItemComponents(typeof(T));

        if (itemComponents == null)
        {
            return null;
        }

        return itemComponents as T[];
    }

    public ItemComponent[] GetItemComponents(Type type)
    {
        TryCacheItemComponents();

        if (type == null)
        {
            return null;
        }

        if (!_itemComponentTypeDictionary.ContainsKey(type))
        {
            return null;
        }

        return _itemComponentTypeDictionary[type];
    }

    public bool TryGetItemComponent(string name, out ItemComponent itemComponent, bool formatName = false)
    {
        TryCacheItemComponents();

        if (formatName)
        {
            name = name.ToLower().Trim();
        }

        return _itemComponentNameDictionary.TryGetValue(name, out itemComponent);
    }

    public bool TryGetItemComponent<T>(out T itemComponent) where T : ItemComponent
    {
        bool success = TryGetItemComponent(typeof(T), out ItemComponent result);

        if (success)
        {
            itemComponent = result as T;
        }
        else
        {
            itemComponent = null;
        }

        return success;
    }

    public bool TryGetItemComponent(Type type, out ItemComponent itemComponent)
    {
        bool success = TryGetItemComponents(type, out ItemComponent[] result);

        if (success)
        {
            itemComponent = result[0];
        }
        else
        {
            itemComponent = null;
        }

        return success;
    }

    public bool TryGetItemComponents<T>(out T[] itemComponents) where T : ItemComponent
    {
        bool success = TryGetItemComponents(typeof(T), out ItemComponent[] result);

        if (success)
        {
            itemComponents = result as T[];
        }
        else
        {
            itemComponents = null;
        }

        return success;
    }

    public bool TryGetItemComponents(Type type, out ItemComponent[] itemComponents)
    {
        TryCacheItemComponents();

        return _itemComponentTypeDictionary.TryGetValue(type, out itemComponents);
    }

    public bool HasItemComponent<T>()
    {
        return HasItemComponent(typeof(T));
    }

    public bool HasItemComponent(Type type)
    {
        TryCacheItemComponents();

        return _itemComponentTypeDictionary.ContainsKey(type);
    }

    public bool HasItemComponent(string name, bool formatName = false)
    {
        TryCacheItemComponents();

        if (formatName)
        {
            name = name.ToLower().Trim();
        }

        return _itemComponentNameDictionary.ContainsKey(name);
    }
    #endregion
}
