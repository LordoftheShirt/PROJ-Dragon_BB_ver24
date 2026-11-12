using UnityEngine;


// This is a bootstrapper that will load the Systems prefab before any scene loads.
// The systems prefab should contain managers that are needed for the game to function. 
// It is used to ensure that the Systems prefab is always present in the scene (even if the scene is loaded directly from the editor).
public static class BootStrapper 
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Execute()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
    }
}
