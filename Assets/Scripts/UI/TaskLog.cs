using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TaskLog : MonoBehaviour
{
    private InputAction openLogAction;
    GameObject taskMenu;

    [SerializeField] private Item mapItem;
    [SerializeField] private GameObject mapStrike;

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

        mapStrike.SetActive(false);
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

        Inventory.OnItemChanged += OnItemChanged;
    }

    private void OnDisable()
    {
        Inventory.OnItemChanged -= OnItemChanged;
    }

    private void OnItemChanged(Item item, int count, int change)
    {
        if (item == mapItem && change > 0)
        {
            mapStrike.SetActive(true);
        }
    }
}
