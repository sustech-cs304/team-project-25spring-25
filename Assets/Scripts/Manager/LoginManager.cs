using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
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
            /*
         * 登录不成功报提示信息、
         * 登录成功先进入选择游戏模式菜单（隐藏login层元素，显示menu层元素）
         * menu层包括（两个按钮【单人模式、多人模式】、右上角设置按钮、排行榜按钮）
         * 单人模式按照该代码切换摄像机、多人模式则按钮下飞出两个小按钮（创建房间、加入房间）
         */
            if (id == "123" && password == "123")
            {
                // 登录成功，调用摄像机切换器切换摄像机
                cameraSwitcher.OnLoginSuccess();
                HideLoginPanel();
                GameManager.Instance.InitRace();
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
}