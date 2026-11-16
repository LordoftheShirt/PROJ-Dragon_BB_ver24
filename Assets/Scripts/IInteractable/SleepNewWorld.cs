using UnityEngine;

public class SleepNewWorld : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        gameObject.GetComponent<SquashAndStretch>().Animate();

        ReferenceManager.Instance.gameManager.ResetWorld();
    }

    public string GetDescription()
    {
        return "Sleep (Generate New World)";
    }
}
