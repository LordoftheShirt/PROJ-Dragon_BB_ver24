using UnityEngine;
using UnityEngine.InputSystem;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private int maxFlags = 5;
    [SerializeField] private float buildRange = 8f;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private InputActionReference placeFlagAction;

    private int currentFlags;
    private Transform builderSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentFlags = maxFlags;
        builderSource = gameObject.transform;
    }

    // Currently the flag counter does not go up when reecollected.
    private void OnPlaceFlagPressed(InputAction.CallbackContext context)
    {
        Ray placementRange = new Ray(builderSource.position, builderSource.forward);
        if (Physics.Raycast(placementRange, out RaycastHit hitinfo, buildRange))
        {
            if (currentFlags <= 0)
            {
                Debug.Log("No flags left to place.");
                return;
            }

            Quaternion flagRotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);

            Instantiate(flagPrefab, hitinfo.point, flagRotation);
            currentFlags--;
        }
    }

    private void OnEnable()
    {
        placeFlagAction.action.Enable();
        placeFlagAction.action.performed += OnPlaceFlagPressed;
    }

    private void OnDisable()
    {
        placeFlagAction.action.Disable();
        placeFlagAction.action.performed -= OnPlaceFlagPressed;
    }
}
