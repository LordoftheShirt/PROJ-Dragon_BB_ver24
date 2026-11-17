using UnityEngine;

public enum ResourceType
{
    Wood = 0,
    /// <summary>
    /// AKA. Metal
    /// </summary>
    Scrap,
    Fabric,
    Magnet,
    Tech,
    Crystal,
    // TODO: more resource types!!
}

public static class ResourceTypeExtensions
{
    public static int GetCountInInventory(this ResourceType type)
    {
        return Inventory.Instance.GetResource(type);
    }

    public static string GetDisplayString(this ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return "Wood";

            case ResourceType.Scrap:
                return "Metal Scrap";

            default:
                return "Unknown";
        }
    }

    public static int GetIndex(this ResourceType resourceType)
    {
        return (int)resourceType;
    }

    public static Sprite GetIcon(this ResourceType resourceType)
    {
        return EnumIcons.GetResourceIcon(resourceType);
    }
}