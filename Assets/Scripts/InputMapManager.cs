using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapManager : Singleton<InputMapManager>
{
    private InputActionMap playerMap;
    private StarterAssetsInputs starterAssetsInputs;
    private GameObject worldUI;

    //
    private BuildingPlacer buildingPlacer;


    private void Start()
    {
        playerMap = InputSystem.actions.FindActionMap("Player");

        GameObject player = GameObject.FindWithTag("Player");
        if(player != null)
        {
            if (player.TryGetComponent<StarterAssetsInputs>(out StarterAssetsInputs sai))
            starterAssetsInputs = sai;

            if (player.TryGetComponent<BuildingPlacer>(out BuildingPlacer bp))
                buildingPlacer = bp;
        }

        worldUI = GameObject.FindWithTag("WorldUI");

        //Debug.Log("WorldUI: " + worldUI.name);
        //Debug.Log("BP: " + buildingPlacer.name);
    }

    public void EnablePlayerMap()
    {
        playerMap.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.enabled = true;
        }

        if (worldUI != null)
        {
            worldUI.SetActive(true);
        }
    }

    public void DisablePlayerMap()
    {
        playerMap.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.enabled = false;
        }

        if (worldUI != null)
        {
            worldUI.SetActive(false);
        }

        if (buildingPlacer != null)
        {
            buildingPlacer.UpdateCurrentGhost(null);
        }
    }



}
