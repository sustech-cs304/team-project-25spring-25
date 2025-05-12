using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public Camera loginCamera; // 登录时使用的摄像机
    public Camera Camera; // 游戏时使用的摄像机
    public Transform target; // 登录摄像机旋转的目标点
    public float rotationSpeed = 30f; // 旋转速度
    public float switchTime = 1f; // 切换时间

    private bool isLoginSuccess = false;
    private float switchTimer;

    void Update()
    {
        if (!isLoginSuccess)
        {
            // 登录成功前，使摄像机绕目标点旋转
            loginCamera.transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
            Camera.enabled=false;
        }
        else
        {
            // 登录成功后，开始切换到游戏摄像机
            if (switchTimer < switchTime)
            {
                switchTimer += Time.deltaTime;
                var t = switchTimer / switchTime;
                loginCamera.transform.position = Vector3.Lerp(loginCamera.transform.position, Camera.transform.position, t);
                loginCamera.transform.rotation = Quaternion.Lerp(loginCamera.transform.rotation, Camera.transform.rotation, t);
            }
            else
            {
                // 切换完成，禁用登录摄像机，启用游戏摄像机
                loginCamera.enabled = false;
                Camera.enabled = true;
            }
        }
    }

    public void OnLoginSuccess()
    {
        isLoginSuccess = true;
        switchTimer = 0f;
    }
}