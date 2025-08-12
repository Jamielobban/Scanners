using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public PlayerWeapon weapon;
    public Vector2 rawInput;
    private Vector2 smoothInput;
    private bool jumpQueued;

    public float speed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float inputSmoothTime = 0.1f;

    private CharacterController controller;
    private float verticalVelocity;

    public Transform cameraTransform;

    // For velocity & acceleration tracking
    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    public Vector3 CurrentVelocity { get; private set; }
    public Vector3 Acceleration { get; private set; }

    public float acceleration = 20f;
    public float deceleration = 25f;
    public float fallMultiplier = 2.5f;  // How much faster you fall

    public MMF_Player shootFeedbacks;
    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        lastPosition = transform.position;
        lastVelocity = Vector3.zero;

        // Subscribe to InputManager events if it exists
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput += OnMoveInput;
            InputManager.Instance.OnJumpInput += OnJumpInput;
            InputManager.Instance.OnAttackInput += OnAttackInput;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= OnMoveInput;
            InputManager.Instance.OnJumpInput -= OnJumpInput;
            InputManager.Instance.OnAttackInput -= OnAttackInput;
        }
    }

    private void Update()
    {
        SmoothInput();
        Vector3 move = CalculateMovement();

        // Step 6: Rotate player toward movement direction (optional)
        if (smoothInput.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camRight * smoothInput.x + camForward * smoothInput.y;

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        HandleJumpAndGravity();
        MoveCharacter(move);

        TrackVelocityAndAcceleration();
    }


    private void SmoothInput()
    {
        float speedChange = (rawInput.magnitude > 0.01f) ? acceleration : deceleration;
        smoothInput = Vector2.MoveTowards(smoothInput, rawInput, speedChange * Time.deltaTime);
        if (smoothInput.magnitude > 1f)
            smoothInput = smoothInput.normalized;
    }

    private Vector3 CalculateMovement()
    {
        if (cameraTransform == null)
        {
            Vector3 fallbackMove = transform.right * smoothInput.x + transform.forward * smoothInput.y;
            return fallbackMove * speed;
        }

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 move = camRight * smoothInput.x + camForward * smoothInput.y;
        return move * speed;
    }

    private void HandleJumpAndGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = 0f;

        if (jumpQueued && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }

        // Faster falling
        if (verticalVelocity < 0)
        {
            verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }


    private void MoveCharacter(Vector3 horizontalMove)
    {
        Vector3 move = horizontalMove;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    private void TrackVelocityAndAcceleration()
    {
        Vector3 currentPosition = transform.position;
        // Calculate velocity = delta position / delta time
        Vector3 velocity = (currentPosition - lastPosition) / Time.deltaTime;

        CurrentVelocity = velocity;

        // Calculate acceleration = delta velocity / delta time
        Acceleration = (CurrentVelocity - lastVelocity) / Time.deltaTime;

        lastVelocity = CurrentVelocity;
        lastPosition = currentPosition;
    }

    // Event handlers for InputManager events
    private void OnMoveInput(Vector2 movementInput)
    {
        rawInput = movementInput;
    }

    private void OnJumpInput()
    {
        jumpQueued = true;
    }

    private void OnAttackInput()
    {
        weapon?.Shoot();
    }


    // Optional if you want to set input manually elsewhere
    public void SetRawInput(Vector2 input)
    {
        rawInput = input;
    }

    public void QueueJump()
    {
        jumpQueued = true;
    }
}
