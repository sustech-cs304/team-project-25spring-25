using Cinemachine;
using UnityEngine;

namespace Manager
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private GameObject playerCamera;

        public void SetPlayerCamera(Transform carTransform)
        {
            playerCamera.GetComponent<CinemachineFreeLook>().LookAt = carTransform;
            playerCamera.GetComponent<CinemachineFreeLook>().Follow = carTransform;
        }
    }
}