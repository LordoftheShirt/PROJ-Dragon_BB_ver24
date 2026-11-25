using System;
using UnityEngine;

public class PlayerUpgrades : Singleton<PlayerUpgrades>
{
    public float SpeedBoost { get; set; } = 0;
    public float ColdResistBoost { get; set; } = 0;

    private void OnEnable()
    {
        Inventory.OnItemChanged += OnItemChanged;
    }

    private void OnDisable()
    {
        Inventory.OnItemChanged -= OnItemChanged;
    }

    private void Start()
    {
        ProcessUpgrades();
    }

    private void OnItemChanged(Item item, int count, int change)
    {
        if (item.HasItemComponent<ItemUpgrade>())
        {
            ProcessUpgrades();
        }
    }

    public void ProcessUpgrades()
    {
        Inventory inv = Inventory.Instance;

        if (inv == null) return;

        SpeedBoost = 0;
        ColdResistBoost = 0;

        foreach (var item in AssetStorage.GetAssetsOfType<Item>())
        {
            // Item is not in inventory
            if (!inv.HasItem(item) || item == null) continue;

            foreach (var component in item.ItemComponents)
            {
                if (component is ItemUpgrade)
                {
                    (component as ItemUpgrade).ApplyUpgrade(this);
                }
            }
        }
    }
}
