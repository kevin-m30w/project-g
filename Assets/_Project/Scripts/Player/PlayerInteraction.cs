using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableMask;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputReader inputReader;

    private IInteractable _currentInteractable;

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.InteractEvent += HandleInteract;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.InteractEvent -= HandleInteract;
        }
    }

    private void Update()
    {
        CheckForInteractable();
    }
    
    private void CheckForInteractable()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            // Check if object (or parent) has an IInteractable component
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                _currentInteractable = interactable;
                 Debug.Log($"Looking at: {interactable.GetInteractPrompt()}");
                return;
            }
        }

        // Reset if raycast hits nothing or non-interactable object
        _currentInteractable = null;
    }

    private void HandleInteract()
    {
        Debug.Log("Interact method called on item!");

        if (_currentInteractable != null)
        {
            PlayerController player = GetComponentInParent<PlayerController>();
            _currentInteractable.Interact(player);
        }

    }
}
