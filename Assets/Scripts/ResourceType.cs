using System.Collections.Generic;
using System.Linq;
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
    private static ResourceType[] _allResourceTypes = null;

    public static ResourceType[] GetAllResourceTypes()
    {
        if (_allResourceTypes == null)
        {
            List<ResourceType> list = new((ResourceType[])System.Enum.GetValues(typeof(ResourceType)));

            list.Sort(new ResourceTypeComparer());

            _allResourceTypes = list.ToArray();
        }

        return _allResourceTypes;
    }

    public static int GetCountInInventory(this ResourceType type)
    {
        return Inventory.Instance.GetResource(type);
    }

    public static string GetDisplayString(this ResourceType resourceType, bool plural = false)
    {
        switch (resourceType)
        {
            case ResourceType.Scrap:
                return plural ? "Metal Scraps" : "Metal Scrap";

            case ResourceType.Wood:
                return "Wood";

            case ResourceType.Fabric:
                return "Fabric";

            case ResourceType.Magnet:
                return "Magnetite";

            case ResourceType.Tech:
                return plural ? "Electronics" : "Electronic";

            case ResourceType.Crystal:
                return plural ? "Thermal Crystals" : "Thermal Crystal";

            default:
                return "Unknown";
        }
    }

    public static int GetSortingOrder(this ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Scrap:
                return 1;

            case ResourceType.Wood:
                return 0;

            case ResourceType.Fabric:
                return 2;

            case ResourceType.Magnet:
                return 3;

            case ResourceType.Tech:
                return 4;

            case ResourceType.Crystal:
                return 5;

            default:
                return -1;
        }
    }

    public static int GetIndex(this ResourceType resourceType)
    {
        return (int)resourceType;
    }

    public static Sprite GetIcon(this ResourceType resourceType)
    {
        return EnumStyle.GetResourceIcon(resourceType);
    }
    public static Color GetColor(this ResourceType resourceType)
    {
        return EnumStyle.GetResourceColor(resourceType);
    }
    public static Color GetBackgroundColor(this ResourceType resourceType)
    {
        return EnumStyle.GetResourceBackgroundColor(resourceType);
    }
}

public class ResourceTypeComparer : IComparer<ResourceType>
{
    public int Compare(ResourceType x, ResourceType y)
    {
        return x.GetSortingOrder().CompareTo(y.GetSortingOrder());
    }
}
