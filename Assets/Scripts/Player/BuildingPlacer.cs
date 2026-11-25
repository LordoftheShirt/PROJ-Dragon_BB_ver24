using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// A raycast you place on the camera; it looks for IInteractable objects.
public class BuildingPlacer : Singleton<BuildingPlacer>
{
    public ConsumablesManager Consumables => ConsumablesManager.Instance;

    [SerializeField] private float buildRange = 8f;
    [SerializeField] private float maxSurfaceAngle = 45f; // Maximum angle from the up vector that is considered a valid surface for placement.

    [Header("Flag")]
    [SerializeField] private GameObject flagGhostPrefab;
    [SerializeField] private GameObject flagPrefab;

    [Header("Fire Kit")]
    [SerializeField] private GameObject fireKitGhostPrefab;
    [SerializeField] private GameObject fireKitPrefab;

    
    [Header("Tent")]
    [SerializeField] private GameObject tentGhostPrefab;
    [SerializeField] private GameObject tentPrefab;


    [Header("Particles (Optional)")]
    [SerializeField] private GameObject placedExplosion;

    [Header("Material (Optional)")]
    [SerializeField] private Material matUnplaceable;

    [Header("LayerMask")]
    [SerializeField] private LayerMask whatIsGround;

    private Material matPlaceable;
    private Renderer rendCurrentMat;

    private float holdToPlaceSec = 1f;
    
    private InputAction confirmBuildAction;
    /* Input handling has been moved to Hotbar script
    private InputAction placeFireKitAction;
    private InputAction placeTentAction;
    private InputAction placeFlagAction;
    */
    private InputAction cancelBuildAction;

    private bool isHoldingBuild = false;
    private float holdTimer = 0f;
    private GameObject currentGhost;
    private Transform builderSource;

    private GameObject buildingHolder;

    // For Placeholder UI.
    public event Action<bool> OnGhostPresent;

    protected override void Awake()
    {
        base.Awake();

        confirmBuildAction = InputSystem.actions["Confirm"];

        /* Input handling has been moved to Hotbar script
        placeFireKitAction = InputSystem.actions["Place Fire"];
        placeTentAction = InputSystem.actions["Place Tent"];
        placeFlagAction = InputSystem.actions["Place Flag"];
        */

        cancelBuildAction = InputSystem.actions["Cancel Build"];


    }

    void Start()
    {
        builderSource = Camera.main.transform;

        ResetBuildings();
    }

    void Update()
    {
        Debug.DrawRay(builderSource.position, builderSource.forward * buildRange, Color.red);

        PreviewGhostLocation();

        bool canPlace = true;

        // prevents ghosts from being placed if it's the wrong material.
        if (rendCurrentMat != null && matUnplaceable != null && rendCurrentMat.sharedMaterial == matUnplaceable)
        {
            canPlace = false;
        }

        // When preview build is active and the player is holding the confirm build button, this counts until holdToPlaceSec is reached, at which point the build is placed.
        if (isHoldingBuild)
        {
            // Flag is placed instantly, no hold needed
            if (canPlace && currentGhost.CompareTag(flagGhostPrefab.tag))
            {
                if (PlaceBuild(flagPrefab, Consumables.GetCurrentFlags()) != null)
                {
                    Consumables.UseFlag();

                    // Flag doesn't remove its ghost unless this was the last flag you placed
                    if (Consumables.GetCurrentFlags() <= 0)
                    {
                        Destroy(currentGhost);
                        currentGhost = null;

                        UnselectHotbar();
                    }

                    isHoldingBuild = false;
                    holdTimer = 0f;
                    return;
                }
            }

            if (canPlace)
            {
                holdTimer += Time.deltaTime;
            }
            else
            {
                holdTimer = 0;
            }

            if (currentGhost == null)
            {
                isHoldingBuild = false;
                holdTimer = 0f;
                return;
            }

            if (canPlace && holdTimer >= holdToPlaceSec)
            {
                if (currentGhost.CompareTag(fireKitGhostPrefab.tag))
                {
                    if (PlaceBuild(fireKitPrefab, Consumables.GetCurrentFirekits(), true) != null)
                    {
                        Consumables.UseFirekit();
                        if (Consumables.GetCurrentFirekits() <= 0) UnselectHotbar();
                    }
                }
                else if (currentGhost.CompareTag(tentGhostPrefab.tag))
                {
                    if (PlaceBuild(tentPrefab, Consumables.GetCurrentTents(), true) != null)
                    {
                        Consumables.UseTent();
                        if (Consumables.GetCurrentTents() <= 0) UnselectHotbar();
                    }
                }

                Destroy(currentGhost);
                currentGhost = null;

                isHoldingBuild = false; // Reset holding state
                holdTimer = 0f; // Reset timer
            }
        }
    }

    /*
    // Place the flag instantly.
    public void PlaceFlag()
    {
        RemoveGhost();

        if (PlaceBuild(flagPrefab, Consumables.GetCurrentFlags()) != null)
            Consumables.UseFlag();
    }
    */

    // Flag has a ghost but is placed down instantly and doesn't need to be held down unlike the other builds
    public void SetGhostToFlag()
    {
        UpdateCurrentGhost(flagGhostPrefab, Consumables.GetCurrentFlags());
    }

    public void SetGhostToFireKit()
    {
        UpdateCurrentGhost(fireKitGhostPrefab, Consumables.GetCurrentFirekits());
    }

    public void SetGhostToTent()
    {
        UpdateCurrentGhost(tentGhostPrefab, Consumables.GetCurrentTents());
    }

    public void RemoveGhost()
    {
        UpdateCurrentGhost(null);
    }

    // Keeps track of the current ghost's location. Locks the location if the player begins confirming the build.
    private void PreviewGhostLocation()
    {
        if (currentGhost != null && !isHoldingBuild)
        {

            Ray placementRange = new Ray(builderSource.position, builderSource.forward);


            // CURRENT BUG 1: must prevent the ghost mesh from clipping into walls.
            // CURRENT BUG 2: must prevent object stacking (need a way to tell "whatIsGround"); this is a problem for the placeBuild() function as well.
            if (Physics.Raycast(placementRange, out RaycastHit hitinfo, buildRange))
            {
                float surfaceAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                Vector3 lookPos = builderSource.position - hitinfo.point;
                lookPos.y = 0;

                currentGhost.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal) * Quaternion.LookRotation(lookPos);
                currentGhost.transform.position = hitinfo.point;

                // changes the ghost's material; its material determines whether it should be allowed to be placed or not.

                if (matPlaceable != null && matUnplaceable != null)
                {
                    bool isValidAngle = surfaceAngle <= maxSurfaceAngle;
                    bool isGroundLayer = (whatIsGround.value & (1 << hitinfo.collider.gameObject.layer)) != 0;

                    if (isValidAngle && isGroundLayer)
                    {
                        if (rendCurrentMat.sharedMaterial != matPlaceable)
                            rendCurrentMat.sharedMaterial = matPlaceable;
                    }
                    else
                    {
                        if (rendCurrentMat.sharedMaterial != matUnplaceable)
                            rendCurrentMat.sharedMaterial = matUnplaceable;
                    }
                }
            }
        }
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

    // unselects equipped item in hotbar upon right click
    private void OnCancelBuild(InputAction.CallbackContext context)
    {
        UnselectHotbar();

        UpdateCurrentGhost(null);
    }

    private void UnselectHotbar()
    {
        if (Hotbar.SelectedSlot.HasValue)
            Hotbar.Instance.SetSelectedSlot(null, false);
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

            // SFX for tent and fireplace here.

            if (placedExplosion != null)
                Instantiate(placedExplosion, currentGhost.transform.position, currentGhost.transform.rotation, buildingHolder.transform);

            return Instantiate(build, currentGhost.transform.position, currentGhost.transform.rotation, buildingHolder.transform);
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

            if(build == flagPrefab)
            {
                // SFX for flag here.
            }

            return Instantiate(build, hitinfo.point, buildRotation, buildingHolder.transform);
        }
        
        // added in case the raycast (when spawning the preview build) never hits anything.
        if(build == flagGhostPrefab || build == fireKitGhostPrefab || build == tentGhostPrefab)
            return Instantiate(build, buildingHolder.transform.position, Quaternion.identity, buildingHolder.transform);

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

        if (currentGhost != null)
        {
            rendCurrentMat = currentGhost.GetComponentInChildren<Renderer>();
            matPlaceable = rendCurrentMat.sharedMaterial;
        }

    }


    private void OnEnable()
    {
        /* Input handling has been moved to Hotbar script
        placeFlagAction.performed += OnPlaceFlagKey;

        placeFireKitAction.performed += OnPlaceFireKitKey;

        placeTentAction.performed += (context) => UpdateCurrentGhost(tentGhostPrefab, Consumables.GetCurrentTents());
        */

        confirmBuildAction.started += OnConfirmBuild;
        confirmBuildAction.canceled += OnConfirmBuild;

        cancelBuildAction.performed += OnCancelBuild;
    }

    private void OnDisable()
    {
        /* Input handling has been moved to Hotbar script
        placeFlagAction.performed -= OnPlaceFlagKey;

        placeFireKitAction.performed -= OnPlaceFireKitKey;

        placeTentAction.performed -= (context) => UpdateCurrentGhost(tentGhostPrefab, Consumables.GetCurrentTents());
        */
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

    public void ResetBuildings()
    {
        if (buildingHolder == null)
        {
            buildingHolder = new GameObject("Building Holder");
        }
        else
        {
            Destroy(buildingHolder);
            buildingHolder = new GameObject("Building Holder");
        }

        // måste reset input/UI build selection hos ghost Preview här. Annars blir den lite skum efter man har sovit.
    }
}
