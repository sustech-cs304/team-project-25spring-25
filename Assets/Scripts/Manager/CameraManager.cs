using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace Manager
{
    public class CameraManager : Singleton<CameraManager>
    {
        public Camera loginCamera;
        public Transform loginTarget; // 登录界面旋转目标
        public float rotationSpeed = 30f;
        public float switchTime = 1f;
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject carCamera;
        private int cameraMode;
        private float switchTimer;

        void Start()
        {
            cameraMode = 1;
        }
        void Update()
        {
            if (cameraMode == 0)
            {
                // 平滑切换到游戏摄像机
                if (switchTimer < switchTime)
                {
                    switchTimer += Time.deltaTime;
                    var t = switchTimer / switchTime;
                    loginCamera.transform.position = Vector3.Lerp(loginCamera.transform.position, camera.transform.position, t);
                    loginCamera.transform.rotation = Quaternion.Lerp(loginCamera.transform.rotation, camera.transform.rotation, t);
                }
                else
                {
                    // 切换完成，禁用登录摄像机，启用游戏摄像机
                    loginCamera.enabled = false;
                    camera.enabled = true;
                }
            }
            else if (cameraMode == 1)
            {
                // 登录摄像机旋转展示
                loginCamera.transform.RotateAround(loginTarget.position, Vector3.up, rotationSpeed * Time.deltaTime);
                camera.enabled = false;
            }
            else if (cameraMode == 2)
            {
                
            }
        }

        public void OnLoginSuccess()
        {
            cameraMode = 0;
            switchTimer = 1f;
        }
        public void OnGarage()
        {
            loginCamera.transform.position = new Vector3(0, 2, -6);
            loginCamera.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            cameraMode = 2;
        }
        public void OnLogin()
        {
            cameraMode = 1;
        }
        public void SetPlayerCamera(Transform carTransform)
        {
            var cinemachine = carCamera.GetComponent<CinemachineFreeLook>();
            if (cinemachine == null) return;
            cinemachine.LookAt = carTransform;
            cinemachine.Follow = carTransform;
        }
    }
}
