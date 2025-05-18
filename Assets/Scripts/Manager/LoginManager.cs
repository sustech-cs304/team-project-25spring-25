using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class LoginManager : Singleton<LoginManager>
    {
        public TMP_Text idLabel; // "Student ID"或"User name"的文本标签
        public TMP_InputField idInput; // 用户ID输入框
        public TMP_InputField passwordInput; // 密码输入框
        public Button loginButton; // 登录按钮
        public GameObject loginPanel; // 登录界面的父GameObject
        public GameObject gamePanel; // 一些不需要显示的游戏内GameObject
        public Button switchModeButton; // 切换登录/注册模式的按钮
        public GameObject menuPanel;  //菜单面板
        private bool isLoginMode = true; // 当前是否为登录模式

        private void Start()
        {
            // 添加登录按钮点击事件
            loginButton.onClick.AddListener(OnLoginButtonClicked);
            switchModeButton.onClick.AddListener(OnSwitchModeClicked);
            // 初始化UI状态
            UpdateUIMode();
            HideMenuPanel();   // 隐藏菜单界面
            HideGamePanel();
            idInput.text = "123";
            passwordInput.text = "123";
        }

        private void OnDestroy()
        {
            // 移除登录按钮点击事件
            loginButton.onClick.RemoveListener(OnLoginButtonClicked);
            switchModeButton.onClick.RemoveListener(OnSwitchModeClicked);
        }

        public void OnLoginButtonClicked()
        {
            // 简单的登录验证
            /*
         * 登录不成功报提示信息、
         * 登录成功先进入选择游戏模式菜单（隐藏login层元素，显示menu层元素）
         * menu层包括（两个按钮【单人模式、多人模式】、右上角设置按钮、排行榜按钮）
         * 单人模式按照该代码切换摄像机、多人模式则按钮下飞出两个小按钮（创建房间、加入房间）
         */
            if (isLoginMode)
            {
                string id = idInput.text;
                string password = passwordInput.text;
                if (id == "123" && password == "123")
                {
                    OnLoginSuccess();
                }
                else
                {
                    Debug.LogError("Invalid ID or Password");
                }
            }
            else
            {
                string username = idInput.text;
                string password = passwordInput.text;
                // 注册逻辑 - 这里可以添加你的注册逻辑
                Debug.Log("Register with username: " + username + " and password: " + password);
                // 注册成功后可以自动切换到登录模式
                isLoginMode = true;
                UpdateUIMode();
            }
        }
        
        private void OnLoginSuccess()
        {
            HideLoginPanel();
            ShowMenuPanel();  // 显示菜单界面
            
            // 不切换摄像机，保持原状
            // 不初始化游戏，保持未开始状态
            
            Debug.Log("登录成功，已切换到菜单界面");
        }
        
        public void OnSwitchModeClicked()
        {
            // 切换模式
            isLoginMode = !isLoginMode;
            UpdateUIMode();
            
            // 清空输入框
            idInput.text = "";
            passwordInput.text = "";
        }
        
        private void UpdateUIMode()
        {
            idInput.text = "";
            passwordInput.text = "";
            if (isLoginMode)
            {
                // 登录模式
                idLabel.text = "Student ID";
                loginButton.GetComponentInChildren<TMP_Text>().text = "  Login";
                switchModeButton.GetComponentInChildren<TMP_Text>().text = "No account? Register";
            }
            else
            {
                // 注册模式
                idLabel.text = "User name";
                loginButton.GetComponentInChildren<TMP_Text>().text = "Register";
                switchModeButton.GetComponentInChildren<TMP_Text>().text = "Login";
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
        
        private void ShowLoginPanel() => loginPanel.SetActive(true);

        private void ShowGamePanel()
        {
            gamePanel.SetActive(true);
        }
        private void ShowMenuPanel() => menuPanel.SetActive(true);
        private void HideMenuPanel() => menuPanel.SetActive(false);
    }
}