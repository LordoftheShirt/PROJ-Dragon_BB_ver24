using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// A raycast you place on the camera; it looks for IInteractable objects.
public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private float buildRange = 8f;

    [Header("Flag")]
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private InputActionReference placeFlagAction;

    [Header("Fire Kit")]
    [SerializeField] private GameObject fireKitGhostPrefab;
    [SerializeField] private GameObject fireKitPrefab;
    [SerializeField] private InputActionReference placeFireKitAction;

    
    [Header("Tent")]
    [SerializeField] private GameObject tentGhostPrefab;
    [SerializeField] private GameObject tentPrefab;
    [SerializeField] private InputActionReference placeTentAction;

    [Header("Preview Build")]

    [SerializeField] private float holdToPlaceSec;
    [SerializeField] private InputActionReference confirmBuildAction;
    [SerializeField] private InputActionReference cancelBuildAction;

    private float holdTimer;
    private GameObject currentGhost;
    private Transform builderSource;

    // For Placeholder UI.
    public event Action<bool> OnGhostPresent;

    void Start()
    {
        builderSource = gameObject.transform;
    }

    void Update()
    {
        if (currentGhost != null)
        {
            Ray placementRange = new Ray(builderSource.position, builderSource.forward);

            if (Physics.Raycast(placementRange, out RaycastHit hitinfo, buildRange))
            {
                currentGhost.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
                currentGhost.transform.position = hitinfo.point;
            }
        }

        Debug.DrawRay(builderSource.position, builderSource.forward * buildRange, Color.red);
    }

    // Place the flag instantly.
    private void OnPlaceFlagKey(InputAction.CallbackContext context)
    {
        UpdateCurrentGhost(null);

        if (PlaceBuild(flagPrefab, ConsumablesManager.Instance.GetCurrentFlags()) != null)
            ConsumablesManager.Instance.UseFlag();
    }

    // Spawns a preview "ghost" Firekit
    private void OnPlaceFireKitKey(InputAction.CallbackContext context)
    {
        UpdateCurrentGhost(fireKitGhostPrefab, ConsumablesManager.Instance.GetCurrentFirekits());
    }


    // works, but want to implement a "hold to place" mechanic. Alternatively, double click.
    private void OnConfirmBuild(InputAction.CallbackContext context)
    {

        // TODO: FIGURE OUT HOW I LISTEN TO A HOLD OVER TIME.
        Debug.Log("Confirmed build.");


        if (currentGhost != null)
        {
            if (currentGhost.CompareTag(fireKitGhostPrefab.tag))
            {
                if (PlaceBuild(fireKitPrefab, ConsumablesManager.Instance.GetCurrentFirekits()) != null)
                    ConsumablesManager.Instance.UseFirekit();
            }
            else if (currentGhost.CompareTag(tentGhostPrefab.tag))
            {
                if (PlaceBuild(tentPrefab, ConsumablesManager.Instance.GetCurrentTents()) != null)
                    ConsumablesManager.Instance.UseTent();
            }
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    private void OnCancelBuild(InputAction.CallbackContext context)
    {
        UpdateCurrentGhost(null);
    }

    // implement a surface degree limit parameter to avoid placing on walls and ceilings. Implement a "Ground" tag of sorts to identify only ground.
    private GameObject PlaceBuild(GameObject build, int currentQuantity)
    {
        Ray placementRange = new Ray(builderSource.position, builderSource.forward);
        if (Physics.Raycast(placementRange, out RaycastHit hitinfo, buildRange))
        {
            if (currentQuantity <= 0)
            {
                Debug.Log("No " + build.name + " left to place.");
                return null;
            }

            Quaternion buildRotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);

            
            return Instantiate(build, hitinfo.point, buildRotation);
        }

        return null;
    }


    // Update the current ghost object based on the provided ghost prefab (ghost: a pre-build, when you want to preview the build location).
    // Do no let the ghost object appear if the player has no more of that item to place.
    private void UpdateCurrentGhost(GameObject ghost, int currentQuantity = 1)
    {
        if (currentQuantity <= 0)
        {
            if (currentGhost != null)
                Destroy(currentGhost);
            OnGhostPresent.Invoke(false);
            return;
        }

        if (ghost == null)
        {
            if (currentGhost != null)
                Destroy(currentGhost);
            OnGhostPresent.Invoke(false);
            return;
        }

        if (currentGhost == null)
        {
            // no ghost
            currentGhost = PlaceBuild(ghost, 1);
            OnGhostPresent.Invoke(true);
        }
        else if (!currentGhost.CompareTag(ghost.tag))
        {
            // Debug.Log("the two ghosts are different.");
            Destroy(currentGhost);
            currentGhost = PlaceBuild(ghost, 1);
            OnGhostPresent.Invoke(true);
        }
        else if (currentGhost.CompareTag(ghost.tag))
        {
            //Debug.Log("the two ghosts are the same.");
            Destroy(currentGhost);
            currentGhost = null;
            OnGhostPresent.Invoke(false);
        }
    }


    private void OnEnable()
    {
        placeFlagAction.action.Enable();
        placeFlagAction.action.performed += OnPlaceFlagKey;

        placeFireKitAction.action.Enable();
        placeFireKitAction.action.performed += OnPlaceFireKitKey;

        placeTentAction.action.Enable();
        placeTentAction.action.performed += (context) => UpdateCurrentGhost(tentGhostPrefab, ConsumablesManager.Instance.GetCurrentTents());

        confirmBuildAction.action.Enable();
        confirmBuildAction.action.performed += OnConfirmBuild;

        cancelBuildAction.action.Enable();
        cancelBuildAction.action.performed += OnCancelBuild;
    }

    private void OnDisable()
    {
        placeFlagAction.action.Disable();
        placeFlagAction.action.performed -= OnPlaceFlagKey;

        placeFireKitAction.action.Disable();
        placeFireKitAction.action.performed -= OnPlaceFireKitKey;

        placeTentAction.action.Disable();
        placeTentAction.action.performed -= (context) => UpdateCurrentGhost(tentGhostPrefab, ConsumablesManager.Instance.GetCurrentTents());

        confirmBuildAction.action.Disable();
        confirmBuildAction.action.performed -= OnConfirmBuild;

        cancelBuildAction.action.Disable();
        cancelBuildAction.action.performed -= OnCancelBuild;
    }

    public float GetHoldTimer()
    {
        return holdTimer;
    }

    public float GetHoldToPlaceSeconds()
    {
        return holdToPlaceSec;
    }
}
