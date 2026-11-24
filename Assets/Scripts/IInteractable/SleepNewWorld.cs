using UnityEngine;
using UnityEngine.UI;

public class SleepNewWorld : MonoBehaviour, IInteractable
{
    [SerializeField] private CanvasGroup sleepAnimationGroup;
    [SerializeField] private float sleepAnimationSpeed;

    private float sleepAnimationTime;
    bool inAnimation = false;
    bool sleeping = false;

    public void Interact()
    {
        gameObject.GetComponent<SquashAndStretch>().Animate();
        sleepAnimationTime = 0;
        inAnimation = true;
        sleeping = true;
        sleepAnimationGroup.alpha = 0.5f;

        BuildingPlacer.Instance?.ResetBuildings();
    }

    private void FixedUpdate()
    {
        if (!inAnimation)
            return;

        sleepAnimationTime += Time.fixedDeltaTime;

        if(sleeping)
        {
            sleepAnimationGroup.alpha = (sleepAnimationTime / sleepAnimationSpeed);
            if(sleepAnimationTime >= sleepAnimationSpeed)
            {
                sleepAnimationTime = 0;
                sleeping = false;
                ReferenceManager.Instance.gameManager.ResetWorld();
            }
        }
        else
        {
            sleepAnimationGroup.alpha = 1 - (sleepAnimationTime / sleepAnimationSpeed); 
            if(sleepAnimationTime >= sleepAnimationSpeed)            
                inAnimation = false;            
        }
    }

    public string GetDescription()
    {
        return "Sleep (Generate New World)";
    }
}
