using UnityEngine;

/// <summary>
/// Singleton that holds sprite icons for ResourceType and BlueprintCategory enums
/// </summary>
[LoadOnInit]
public class EnumStyle : SingletonPersistent<EnumStyle>
{
    public static Sprite[] ResourceIcons => Instance.resourceIcons;
    public static Color[] ResourceColors => Instance.resourceColors;
    public static Color[] ResourceBackgroundColors => Instance.resourceBackgroundColors;
    public static Sprite[] BlueprintCategoryIcons => Instance.blueprintCategoryIcons;
    public static Color DisabledColor => Instance.disabledColor;
    public static Color DisabledBackgroundColor => Instance.disabledBackgroundColor;
    [SerializeField] private Sprite[] resourceIcons;
    [SerializeField] private Color[] resourceColors;
    [SerializeField] private Color[] resourceBackgroundColors;
    [SerializeField] private Sprite[] blueprintCategoryIcons;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color disabledBackgroundColor;

    public static Sprite GetResourceIcon(int index)
    {
        return ResourceIcons[index];
    }
    public static Sprite GetResourceIcon(ResourceType type)
    {
        return GetResourceIcon(type.GetIndex());
    }
    public static Color GetResourceColor(int index)
    {
        return ResourceColors[index];
    }
    public static Color GetResourceColor(ResourceType type)
    {
        return GetResourceColor(type.GetIndex());
    }
    public static Color GetResourceBackgroundColor(int index)
    {
        return ResourceBackgroundColors[index];
    }
    public static Color GetResourceBackgroundColor(ResourceType type)
    {
        return GetResourceBackgroundColor(type.GetIndex());
    }

    public static Sprite GetBlueprintCategoryIcon(int index)
    {
        return BlueprintCategoryIcons[index];
    }
    public static Sprite GetBlueprintCategoryIcon(BlueprintCategory category)
    {
        return GetBlueprintCategoryIcon(category.GetIndex());
    }
}
