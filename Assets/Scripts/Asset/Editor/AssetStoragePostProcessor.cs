using System;
using UnityEditor;

public class AssetStoragePostProcessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        // Loop through every imported asset. These assets are either newly added to the project or were changed.
        foreach (string assetPath in importedAssets)
        {
            // We are simply interested in the type of file that was modified, so we use GetMainAssetTypeAtPath instead of loading the asset
            Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            foreach (Type checkType in AssetStorage.AssetTypes)
            {
                if (type == checkType)
                {
                    // Update asset storage
                    AssetStorageEditor.Update();
                    break;
                }
            }
        }
    }
}
