using UnityEngine;

public class ColdResistUpgrade : ItemUpgrade
{
    [SerializeField] private float boost;

    public override void ApplyUpgrade(PlayerUpgrades player)
    {
        player.ColdResistBoost += boost;
    }
}
