using System;
using UnityEngine;
using UnityEngine.UI;

public class SleepNewWorld : MonoBehaviour, IInteractable
{
    [SerializeField] private CanvasGroup sleepAnimationGroup;
    [SerializeField] private float sleepAnimationSpeed;

    private float sleepAnimationTime;
    private bool inAnimation = false;
    private bool sleeping = false;

    private bool isSleepingAtHome = true;

    public void Interact()
    {
        if(isSleepingAtHome)
        {
            HomeSleep();
        }
        else
        {
            // tent-sleep logic could go here; FixedUpdate would primarily need editing.
        }
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
                Inventory.Instance.ConvertTemporaryResourcesToPermanent();
                ReferenceManager.Instance.gameManager.IncreaseDay();
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

    private void HomeSleep()
    {
        if (sleeping) return;

        gameObject.GetComponent<SquashAndStretch>().Animate();
        sleepAnimationTime = 0;
        inAnimation = true;
        sleeping = true;
        sleepAnimationGroup.alpha = 0.5f;

        if (BuildingPlacer.Instance != null)
        {
            BuildingPlacer.Instance.ResetBuildings();
        }

        if (Hotbar.SelectedSlot.HasValue)
            Hotbar.Instance.SetSelectedSlot(null, false);
    }
}
