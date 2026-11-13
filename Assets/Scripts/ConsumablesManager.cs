using System;
using UnityEngine;

// simply tracks the max and current counts of consumables. Does not communicate out to other scripts.
public class ConsumablesManager : Singleton<ConsumablesManager>
{
    [Header("Flags")]
    [SerializeField] private int maxFlags = 5;
    [SerializeField] private int currentFlags = 3;

    [Header("Tents")]
    [SerializeField] private int maxTents = 3;
    [SerializeField] private int currentTents = 2;

    [Header("Firekits")]
    [SerializeField] private int maxFirekits = 3;
    [SerializeField] private int currentFirekits = 2;

    public event Action<int> OnFlagsChanged;
    public event Action<int> OnTentsChanged;
    public event Action<int> OnFirekitsChanged;

    public int GetMaxFlags()
    {
        return maxFlags;
    }

    public int GetMaxTents()
    {
        return maxTents;
    }

    public int GetMaxFirekits()
    {
        return maxFirekits;
    }

    public int GetCurrentFlags()
    {
        return currentFlags;
    }

    public int GetCurrentTents()
    {
        return currentTents;
    }

    public int GetCurrentFirekits()
    {
        return currentFirekits;
    }

    public void AddFlag()
    {
        if (currentFlags < maxFlags)
        {
            currentFlags++;
            OnFlagsChanged?.Invoke(currentFlags);
        }
    }

    public void AddTent()
    {
        if (currentTents < maxTents)
        {
            currentTents++;
            OnTentsChanged?.Invoke(currentTents);
        }
    }

    public void AddFirekit()
    {
        if (currentFirekits < maxFirekits)
        {
            currentFirekits++;
            OnFirekitsChanged?.Invoke(currentFirekits);
        }
    }

    public void UseFlag()
    {
        if (currentFlags > 0)
        {
            currentFlags--;
            OnFlagsChanged?.Invoke(currentFlags);
        }
    }

    public void UseTent()
    {
        if (currentTents > 0)
        {
            currentTents--;
            OnTentsChanged?.Invoke(currentTents);
        }
    }

    public void UseFirekit()
    {
        if (currentFirekits > 0)
        {
            currentFirekits--;
            OnFirekitsChanged?.Invoke(currentFirekits);
        }
    }
}
