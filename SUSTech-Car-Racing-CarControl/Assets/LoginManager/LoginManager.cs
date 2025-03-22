using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField idInput; // 用户ID输入框
    public TMP_InputField passwordInput; // 密码输入框
    public Button loginButton; // 登录按钮
    public CameraController cameraSwitcher; // 摄像机切换器
    public GameObject loginPanel; // 登录界面的父GameObject
    public GameObject gamePanel; // 一些不需要显示的游戏内GameObject

    private void Start()
    {
        // 添加登录按钮点击事件
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        HideGamePanel();
    }

    private void OnDestroy()
    {
        // 移除登录按钮点击事件
        loginButton.onClick.RemoveListener(OnLoginButtonClicked);
    }

    public void OnLoginButtonClicked()
    {
        string id = idInput.text;
        string password = passwordInput.text;

        // 简单的登录验证
        if (id == "12211717" && password == "Sh040416")
        {
            // 登录成功，调用摄像机切换器切换摄像机
            cameraSwitcher.OnLoginSuccess();
            HideLoginPanel();
            ShowGamePanel();
        }
        else
        {
            Debug.LogError("Invalid ID or Password");
        }
    }
    
    private void HideLoginPanel()
    {
        // 隐藏登录界面
        loginPanel.SetActive(false);
    }

    private void HideGamePanel()
    {
        gamePanel.SetActive(false);
    }

    private void ShowGamePanel()
    {
        gamePanel.SetActive(true);
    }
}