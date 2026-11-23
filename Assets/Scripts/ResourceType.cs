using UnityEngine;

public enum ResourceType
{   
    /// <summary>
    /// AKA. Metal
    /// </summary>
    Scrap = 0, 
    Wood = 1,
    Fabric = 2,
    Magnet = 3,
    Tech = 4,
    Crystal = 5,
    // TODO: more resource types!!
}

public static class ResourceTypeUtil
{
    public static ResourceType[] GetAllResourceTypes()
    {
        return (ResourceType[])System.Enum.GetValues(typeof(ResourceType));
    }

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

            case ResourceType.Fabric:
                return "Fabric";

            case ResourceType.Magnet:
                return "Magnet";

            case ResourceType.Tech:
                return "Tech";

            case ResourceType.Crystal:
                return "Crystal";

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