using UnityEngine;

public class SpeedUpgrade : ItemUpgrade
{
    [SerializeField] private float boost;

    public override void ApplyUpgrade(PlayerUpgrades player)
    {
        player.SpeedBoost += boost;
    }
}
