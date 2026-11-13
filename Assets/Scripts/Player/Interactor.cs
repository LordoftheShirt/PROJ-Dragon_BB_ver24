using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    public void Interact();

    public string GetDescription();
}

// A raycast you place on the camera; it looks for IInteractable objects.
public class Interactor : MonoBehaviour
{
    [Header("Essential")]
    [SerializeField] private float interactRange = 5f;

    // UI Text pop up
    [Header("Non-essential, for UI display only")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;

    private Transform interactorSource;
    private InputAction interactAction;
    private IInteractable currentInteractable;


    private void Awake()
    {
        interactAction = InputSystem.actions["Interact"];
    }

    void Start()
    {
        interactorSource = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(interactorSource.position, interactorSource.forward * interactRange, Color.green);

        Ray interactRay = new Ray(interactorSource.position, interactorSource.forward);
        if (Physics.Raycast(interactRay, out RaycastHit hitinfo, interactRange) && hitinfo.collider.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;

            // Visual.
            TextPopUp(true);
        }
        else
        {
            currentInteractable = null;

            TextPopUp(false);
        }
    }

    private void TextPopUp(bool active)
    {
        if (interactionUI != null && interactionText != null)
        {
            interactionUI.SetActive(active);
            if (active)
            {
                interactionText.text = currentInteractable.GetDescription();
            }
        }
    }



    // inputmanager later?
    private void OnInteractPressed(InputAction.CallbackContext context)
    {

            //Debug.Log("Interact Pressed");

            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
    }

    private void OnEnable()
    {
        interactAction.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteractPressed;
    }

    public void SetInteractRange(float range)
    {
        interactRange = range;
    }
}
