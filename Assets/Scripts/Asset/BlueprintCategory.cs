using UnityEngine;

public enum BlueprintCategory
{
    Mobility,
    ColdResistance,
    Navigation,
    Survival
}

public static class BlueprintCategoryExtensions
{
    public static string GetDisplayString(this BlueprintCategory category)
    {
        switch (category)
        {
            case BlueprintCategory.Mobility:
                return "Mobility";

            case BlueprintCategory.ColdResistance:
                return "Cold Resistance";

            case BlueprintCategory.Navigation:
                return "Navigation";

            case BlueprintCategory.Survival:
                return "Survival";

            default:
                return "Unknown";
        }
    }

    public static int GetIndex(this BlueprintCategory category)
    {
        return (int)category;
    }

    public static Sprite GetIcon(this BlueprintCategory category)
    {
        return EnumIcons.GetBlueprintCategoryIcon(category);
    }
}
