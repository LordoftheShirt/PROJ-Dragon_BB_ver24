using UnityEngine;

public class SleepNewWorld : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        // Reset PCGeneration from here.
        ReferenceManager.Instance.terrainGenerator.NewSeed();
        ReferenceManager.Instance.terrainGenerator.CreateNewMap();
    }

    public string GetDescription()
    {
        return "Sleep (Generate New World)";
    }
}
