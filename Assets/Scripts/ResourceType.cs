using UnityEngine;

public enum ResourceType
{
    Wood = 0,
    /// <summary>
    /// AKA. Metal
    /// </summary>
    Scrap,
    NumberOfResourceTypes
    // TODO: more resource types!!
}

public static class ResourceTypeExtensions
{
    public static string GetDisplayString(this ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Wood => "Wood",
            ResourceType.Scrap => "Metal Scrap",
            _ => "Unknown"
        };
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