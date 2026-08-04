using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float sprintStaminaCost = 10f;
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float jumpStaminaCost = 20f;
    [SerializeField] private float mouseSensitivity = 60f;
    [SerializeField] private float maxPitch = 85f;

    [Header("Gravity & Grounding")]
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private PlayerStamina playerStamina;

    private CharacterController _characterController;
    private float _xRotation = 0f;

    private Vector3 _velocity;
    private bool _isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.JumpEvent += HandleJump;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.JumpEvent -= HandleJump;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleRotation();
        HandleMovement();
        ApplyGravity();
    }

    private void HandleGroundCheck()
    {
        // Only perform sphere check if velocity isn't actively shooting upward
        if (_velocity.y <= 0)
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            _isGrounded = false;
        }

        // Keep character snapped to ground when walking on slopes/flat ground
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    }

    private void HandleJump()
    {
        if (_isGrounded)
        {
            if (playerStamina != null && !playerStamina.TryConsumeStamina(jumpStaminaCost))
            {
                Debug.Log("Out of stamina jump!");
                return;
            }

            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _isGrounded = false;
        }
    }

    private void HandleRotation()
    {
        Vector2 look = inputReader.LookInput * (mouseSensitivity * Time.deltaTime);

        _xRotation -= look.y;
        _xRotation = Mathf.Clamp(_xRotation, -maxPitch, maxPitch);

        cameraHolder.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * look.x);
    }

    private void HandleMovement()
    {
        Vector2 input = inputReader.MoveInput;
        bool wantsToSprint = inputReader.IsSprinting && input.magnitude > 0.1f && _isGrounded;

        float currentSpeed = walkSpeed;

        // Check if sprinting and drain stamina over time
        if (wantsToSprint && playerStamina != null)
        {
            if (playerStamina.HasStamina(5f)) // Require at least 5 stamina to maintain sprint
            {
                currentSpeed = sprintSpeed;
                playerStamina.TryConsumeStamina(sprintStaminaCost * Time.deltaTime);
            }
        }

        Vector3 moveDir = transform.right * input.x + transform.forward * input.y;
        _characterController.Move(moveDir * (currentSpeed * Time.deltaTime));
    }

    private void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    // Visual helper to see ground check sphere in Scene view
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            // GREEN = Grounded, RED = In Air
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
        else
        {
            // If groundCheck is unassigned, draw a RED warning sphere at player origin
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}