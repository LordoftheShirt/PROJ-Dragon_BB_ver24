using System;
using UnityEngine;

public abstract class ItemComponent : ScriptableObject
{
#if UNITY_EDITOR
#pragma warning disable 414
    [HideInInspector]
    [SerializeField] private bool expandedInEditor = true;
#pragma warning restore 414

    [HideInInspector]
    [SerializeField] private int orderInEditor;
#endif

    [HideInInspector]
    [SerializeField] protected Item item;
    [HideInInspector]
    [SerializeField] private bool disableOnStart;
    [NonSerialized]
    private bool _enabled;

    internal void InternalInitialize()
    {

    }

    public virtual void Use()
    {

    }

    public virtual void Select()
    {

    }

    public virtual void Unselect()
    {

    }

    #region GetItemComponent Methods
    public T GetItemComponent<T>() where T : ItemComponent => item.GetItemComponent<T>();

    public ItemComponent GetItemComponent(Type type) => item.GetItemComponent(type);

    public T[] GetItemComponents<T>() where T : ItemComponent => item.GetItemComponents<T>();

    public ItemComponent[] GetItemComponents(Type type) => item.GetItemComponents(type);

    public bool TryGetItemComponent<T>(out T itemComponent) where T : ItemComponent => item.TryGetItemComponent(out itemComponent);

    public bool TryGetItemComponent(Type type, out ItemComponent itemComponent) => item.TryGetItemComponent(type, out itemComponent);

    public bool TryGetItemComponents<T>(out T[] itemComponents) where T : ItemComponent => item.TryGetItemComponents(out itemComponents);

    public bool TryGetItemComponents(Type type, out ItemComponent[] itemComponents) => item.TryGetItemComponents(type, out itemComponents);

    public bool HasItemComponent<T>() => item.HasItemComponent(typeof(T));

    public bool HasItemComponent(Type type) => item.HasItemComponent(type);
    #endregion
}
