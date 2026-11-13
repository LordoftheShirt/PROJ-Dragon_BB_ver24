using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// A raycast you place on the camera; it looks for IInteractable objects.
public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private float buildRange = 8f;
    [SerializeField] private float maxSurfaceAngle = 45f; // Maximum angle from the up vector that is considered a valid surface for placement.

    [Header("Flag")]
    [SerializeField] private GameObject flagPrefab;

    [Header("Fire Kit")]
    [SerializeField] private GameObject fireKitGhostPrefab;
    [SerializeField] private GameObject fireKitPrefab;

    
    [Header("Tent")]
    [SerializeField] private GameObject tentGhostPrefab;
    [SerializeField] private GameObject tentPrefab;


    [Header("Preview Build")]

    [SerializeField] private float holdToPlaceSec = 2f;
    
    private InputAction confirmBuildAction;
    private InputAction placeFireKitAction;
    private InputAction placeTentAction;
    private InputAction cancelBuildAction;
    private InputAction placeFlagAction;

    private bool isHoldingBuild = false;
    private float holdTimer = 0f;
    private GameObject currentGhost;
    private Transform builderSource;

    // For Placeholder UI.
    public event Action<bool> OnGhostPresent;

    private void Awake()
    {
        confirmBuildAction = InputSystem.actions["Confirm"];

        placeFireKitAction = InputSystem.actions["Place Fire"];

        placeTentAction = InputSystem.actions["Place Tent"];

        cancelBuildAction = InputSystem.actions["Cancel Build"];

        placeFlagAction = InputSystem.actions["Place Flag"];
    }

    void Start()
    {
        builderSource = Camera.main.transform;
    }

    void Update()
    {
        Debug.DrawRay(builderSource.position, builderSource.forward * buildRange, Color.red);

        // Keeps track of the current ghost's location. Locks the location if the player begins confirming the build.
        if (currentGhost != null && !isHoldingBuild)
        {

            Ray placementRange = new Ray(builderSource.position, builderSource.forward);


            // CURRENT BUG 1: must prevent the ghost mesh from clipping into walls.
            // CURRENT BUG 2: must prevent object stacking (need a way to tell "whatIsGround"); this is a problem for the placeBuild() function as well.
            if (Physics.Raycast(placementRange, out RaycastHit hitinfo, buildRange))
            {
                float surfaceAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                if (surfaceAngle < maxSurfaceAngle)
                {
                    currentGhost.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
                    currentGhost.transform.position = hitinfo.point;
                }
                else
                {
                    // CURRENT BUG 3: ghost spawns can be placed on surfaces that are too steep (during the moment wherein the ghost spawns). Must build an indicator here that shows the player they can't place the ghost on this surface: change the material to red.
                }
            }
        }


        // When preview build is active and the player is holding the confirm build button, this counts until holdToPlaceSec is reached, at which point the build is placed.
        if (isHoldingBuild)
        {
            holdTimer += Time.deltaTime;

            if(currentGhost == null)
            {
                isHoldingBuild = false;
                holdTimer = 0f;
                return;
            }

            if (holdTimer >= holdToPlaceSec)
            {
                if (currentGhost.CompareTag(fireKitGhostPrefab.tag))
                {
                    if (PlaceBuild(fireKitPrefab, ConsumablesManager.Instance.GetCurrentFirekits(), true) != null)
                        ConsumablesManager.Instance.UseFirekit();
                }
                else if (currentGhost.CompareTag(tentGhostPrefab.tag))
                {
                    if (PlaceBuild(tentPrefab, ConsumablesManager.Instance.GetCurrentTents(), true) != null)
                        ConsumablesManager.Instance.UseTent();
                }
                Destroy(currentGhost);
                currentGhost = null;

                isHoldingBuild = false; // Reset holding state
                holdTimer = 0f; // Reset timer
            }
        }
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



    // when a previeww build is selected,the player must hold down the confirm build button for a set amount of time to place it. If the player releases the button before the time is up, the build is not placed.
    private void OnConfirmBuild(InputAction.CallbackContext context)
    {
        if (currentGhost != null)
        {
            //Debug.Log("Confirm build action triggered.");
            if (context.started)
            {
                //Debug.Log("Confirm build action started.");
                isHoldingBuild = true;
                holdTimer = 0f;
            }
            else if (context.canceled)
            {
                //Debug.Log("Confirm build action canceled.");
                isHoldingBuild = false;
                holdTimer = 0f; // Reset timer if released early
            }
        }
    }



    private void OnCancelBuild(InputAction.CallbackContext context)
    {
        UpdateCurrentGhost(null);
    }

    // returns the placed build object if successful.
    private GameObject PlaceBuild(GameObject build, int currentQuantity, bool useGhostLocation = false)
    {
        if (useGhostLocation)
        {
            if (currentGhost == null)
            {
                Debug.Log("No ghost to derive location from?");
                return null;
            }

            return Instantiate(build, currentGhost.transform.position, currentGhost.transform.rotation);
        }


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
    public void UpdateCurrentGhost(GameObject ghost, int currentQuantity = 1)
    {
        holdTimer = 0f; // Reset the hold timer whenever the ghost is updated

        if (currentQuantity <= 0)
        {
            if (currentGhost != null)
                Destroy(currentGhost);
            OnGhostPresent?.Invoke(false);
            return;
        }

        if (ghost == null)
        {
            if (currentGhost != null)
                Destroy(currentGhost);
            OnGhostPresent?.Invoke(false);
            return;
        }

        if (currentGhost == null)
        {
            // no ghost
            currentGhost = PlaceBuild(ghost, 1);
            OnGhostPresent?.Invoke(true);
        }
        else if (!currentGhost.CompareTag(ghost.tag))
        {
            // Debug.Log("the two ghosts are different.");
            Destroy(currentGhost);
            currentGhost = PlaceBuild(ghost, 1);
            OnGhostPresent?.Invoke(true);
        }
        else if (currentGhost.CompareTag(ghost.tag))
        {
            //Debug.Log("the two ghosts are the same.");
            Destroy(currentGhost);
            currentGhost = null;
            OnGhostPresent?.Invoke(false);
        }
    }


    private void OnEnable()
    {
        placeFlagAction.performed += OnPlaceFlagKey;


        placeFireKitAction.performed += OnPlaceFireKitKey;

        placeTentAction.performed += (context) => UpdateCurrentGhost(tentGhostPrefab, ConsumablesManager.Instance.GetCurrentTents());

        confirmBuildAction.started += OnConfirmBuild;
        confirmBuildAction.canceled += OnConfirmBuild;

        cancelBuildAction.performed += OnCancelBuild;
    }

    private void OnDisable()
    {
        placeFlagAction.performed -= OnPlaceFlagKey;

        placeFireKitAction.performed -= OnPlaceFireKitKey;

        placeTentAction.performed -= (context) => UpdateCurrentGhost(tentGhostPrefab, ConsumablesManager.Instance.GetCurrentTents());

        confirmBuildAction.started += OnConfirmBuild;
        confirmBuildAction.canceled += OnConfirmBuild;

        cancelBuildAction.performed -= OnCancelBuild;
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
