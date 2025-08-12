using UnityEngine;

public class WeaponRotationSway : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Rotation Sway Settings")]
    public float swayAmount = 1.5f;      // How much the weapon sways (degrees)
    public float rotationLag = 10f;      // How fast weapon rotation catches up

    private Quaternion lastSmoothedRotation;

    void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("WeaponRotationSway: No camera assigned.");
            enabled = false;
            return;
        }

        lastSmoothedRotation = cameraTransform.rotation;
        transform.localRotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        // Smooth the camera rotation itself
        lastSmoothedRotation = Quaternion.Slerp(lastSmoothedRotation, cameraTransform.rotation, Time.deltaTime * rotationLag);

        // Calculate delta between smoothed and current camera rotation
        Quaternion deltaRotation = cameraTransform.rotation * Quaternion.Inverse(lastSmoothedRotation);

        // Convert to euler angles
        Vector3 deltaEuler = deltaRotation.eulerAngles;
        deltaEuler.x = NormalizeAngle(deltaEuler.x);
        deltaEuler.y = NormalizeAngle(deltaEuler.y);
        deltaEuler.z = NormalizeAngle(deltaEuler.z);

        // Inverted sway effect for inertia
        Vector3 swayEulerAngles = new Vector3(
            -deltaEuler.x * swayAmount,
            -deltaEuler.y * swayAmount,
            0f
        );

        Quaternion desiredLocalRotation = Quaternion.Euler(swayEulerAngles);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredLocalRotation, Time.deltaTime * rotationLag);
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
