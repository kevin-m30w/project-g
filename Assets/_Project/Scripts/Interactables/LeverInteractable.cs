using Unity.VisualScripting;
using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [Header("Lever Settings")]
    [SerializeField] private Transform handleTransform;
    [SerializeField] private Vector3 pulledRotation = new Vector3(45f, 0f, 0f);
    [SerializeField] private Vector3 defaultRotation = new Vector3(-45f, 0f, 0f);

    private bool _isPulled = false;

    public string GetInteractPrompt()
    {
        return _isPulled ? "Reset Lever" : "Pull Lever [E]";
    }

    public void Interact(PlayerController player)
    {
        _isPulled = !_isPulled;

        handleTransform.localRotation = Quaternion.Euler(_isPulled ? pulledRotation : defaultRotation);
        
        if (_isPulled)
        {
            Debug.Log("LEVER PULLED! Triggering game start flow...");
            // Call your LobbyManager.OnLeverPulled() here!
        }
    }
}
