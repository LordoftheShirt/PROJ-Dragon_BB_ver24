using UnityEngine;

/// <summary>
/// Singleton that holds sprite icons for ResourceType and BlueprintCategory enums
/// </summary>
[LoadOnInit]
public class EnumIcons : SingletonPersistent<EnumIcons>
{
    public static Sprite[] ResourceIcons => Instance.resourceIcons;
    public static Sprite[] BlueprintCategoryIcons => Instance.blueprintCategoryIcons;
    [SerializeField] private Sprite[] resourceIcons;
    [SerializeField] private Sprite[] blueprintCategoryIcons;

    public static Sprite GetResourceIcon(int index)
    {
        return ResourceIcons[index];
    }
    public static Sprite GetResourceIcon(ResourceType type)
    {
        return GetResourceIcon(type.GetIndex());
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
