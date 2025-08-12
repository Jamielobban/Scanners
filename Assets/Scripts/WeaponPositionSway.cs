using UnityEngine;

public class WeaponPositionSway : MonoBehaviour
{
    public Vector3 baseLocalPosition = new Vector3(0.3f, -0.25f, 0.5f);

    [Header("Sway Settings")]
    public float swayAmount = 0.05f;      // Max sway offset in units
    public float swaySmoothness = 8f;     // How quickly sway moves to target position
    public float accelerationSwayMultiplier = 0.1f; // How much sway reacts to acceleration

    [Header("References")]
    public PlayerController playerController;  // Reference to your player controller to get move input

    private Vector3 currentSwayOffset = Vector3.zero;

    public Transform copyPosition;

    void Start()
    {
        transform.localPosition = baseLocalPosition;
    }

    void LateUpdate()
    {
        if (playerController == null)
            return;

        Vector2 input = playerController.rawInput;
        Transform cam = playerController.cameraTransform;

        if (cam == null)
            return;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 worldMoveDir = camRight * input.x + camForward * input.y;

        Vector3 targetSway = Vector3.zero;

        // Horizontal sway from movement input
        if (worldMoveDir.magnitude > 0.1f)
        {
            Vector3 localMoveDir = transform.parent.InverseTransformDirection(worldMoveDir);

            targetSway.x = localMoveDir.x * swayAmount;
            targetSway.z = -localMoveDir.z * swayAmount * 0.5f;
        }

        // Vertical sway fake from vertical velocity (jump/fall)
        float verticalVelocity = playerController.CurrentVelocity.y;

        // You can tweak this multiplier for effect strength
        float verticalSway = Mathf.Clamp(verticalVelocity * 0.05f, -swayAmount, swayAmount);

        targetSway.y = verticalSway;

        // Smoothly lerp to target sway offset
        currentSwayOffset = Vector3.Lerp(currentSwayOffset, targetSway, Time.deltaTime * swaySmoothness);

        transform.localPosition = baseLocalPosition + currentSwayOffset;
        copyPosition = transform;
    }
}
