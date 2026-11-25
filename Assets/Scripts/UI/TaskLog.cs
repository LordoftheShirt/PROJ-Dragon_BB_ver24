using UnityEngine;
using UnityEngine.InputSystem;

public class TaskLog : MonoBehaviour
{
    private InputAction openLogAction;
    GameObject taskMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        openLogAction = InputSystem.actions["OpenTaskLog"];
    }

    void Start()
    {
            // Currently, always grabs the second second child.
            taskMenu = gameObject.transform.GetChild(1).gameObject;
            taskMenu.SetActive(false);
    }

    private void OpenLog(InputAction.CallbackContext obj)
    {
        if (taskMenu.activeSelf)
            taskMenu.SetActive(false);
        else
            taskMenu.SetActive(true);

    }

    private void OnEnable()
    {
        openLogAction.started += OpenLog;
    }

    private void OnDisable()
    {
        
    }
}
