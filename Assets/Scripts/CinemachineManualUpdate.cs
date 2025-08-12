using UnityEngine;
using Unity.Cinemachine;
public class CinemachineManualUpdate : MonoBehaviour
{
    CinemachineBrain brain;
    void Start()
    {
        brain = CinemachineBrain.GetActiveBrain(0);
    }
    void LateUpdate()
    {
        brain.ManualUpdate();
    }
}
