using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    public enum Mode { Full, YAxisOnly }
    [Header("Target")]
    public Camera targetCamera;          // If null, uses Camera.main
    [Header("Behavior")]
    public Mode mode = Mode.Full;        // Full = free look, YAxisOnly = upright billboard
    public bool invertForward = false;   // Flip if your quad points the wrong way (+Z vs -Z)

    void LateUpdate()
    {
        var cam = targetCamera ? targetCamera : Camera.main;
        if (!cam) return;

        if (mode == Mode.Full)
        {
            Vector3 toCam = cam.transform.position - transform.position;
            if (invertForward) toCam = -toCam;
            if (toCam.sqrMagnitude > 1e-6f)
                transform.rotation = Quaternion.LookRotation(toCam, Vector3.up);
        }
        else // YAxisOnly (upright)
        {
            Vector3 toCam = cam.transform.position - transform.position;
            toCam.y = 0f; // keep upright
            if (invertForward) toCam = -toCam;
            if (toCam.sqrMagnitude > 1e-6f)
                transform.rotation = Quaternion.LookRotation(toCam, Vector3.up);
        }
    }
}
