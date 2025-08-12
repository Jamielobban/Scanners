using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerControls controls;

    public event Action<Vector2> OnMoveInput;
    public event Action OnJumpInput;
    public event Action OnAttackInput;
    public event Action OnReloadInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMoveInput?.Invoke(Vector2.zero);
        controls.Player.Jump.performed += ctx => OnJumpInput?.Invoke();
        controls.Player.Attack.performed += ctx => OnAttackInput?.Invoke();
        controls.Player.Reload.performed += ctx => OnReloadInput?.Invoke();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() => controls.Player.Enable();

    private void OnDisable() => controls.Player.Disable();
}
