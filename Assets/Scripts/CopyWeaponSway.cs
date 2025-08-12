using UnityEngine;

public class CopyWeaponSway : MonoBehaviour
{
    public Transform pos;
    public Vector3 offset;

    void Update()
    {
       if (pos != null)
        {
            transform.position = pos.position + offset;
        }
    }
}
