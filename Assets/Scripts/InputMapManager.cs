using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapManager : Singleton<InputMapManager>
{
    private InputActionMap playerMap;
    private void Start()
    {
        playerMap = InputSystem.actions.FindActionMap("Player");
    }

    public void EnablePlayerMap()
    {
        playerMap.Enable();
    }

    public void DisablePlayerMap()
    {
        playerMap.Disable();
    }


}
