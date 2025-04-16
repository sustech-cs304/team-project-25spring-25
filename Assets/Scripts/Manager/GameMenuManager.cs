using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class MenuManager : MonoBehaviour
    {
        public Button singleModeButton;    // 单机模式按钮
        public Button multiModeButton;     // 多人模式按钮
        public GameObject menuPanel;       // 当前菜单面板
        public GameObject multiModePanel;  // 多人模式界面
        public CameraController cameraSwitcher; // 摄像机切换控制器
        public GameObject gamePanel;

        private void Start()
        {
            // 确保初始状态正确
            menuPanel.SetActive(true);
            if (multiModePanel != null) multiModePanel.SetActive(false);
            
            // 添加按钮事件监听
            singleModeButton.onClick.AddListener(OnSingleModeClicked);
            multiModeButton.onClick.AddListener(OnMultiModeClicked);
        }

        private void OnDestroy()
        {
            // 移除事件监听
            singleModeButton.onClick.RemoveListener(OnSingleModeClicked);
            multiModeButton.onClick.RemoveListener(OnMultiModeClicked);
        }

        /// <summary>
        /// 点击单机模式按钮的处理
        /// </summary>
        private void OnSingleModeClicked()
        {
            // 隐藏菜单界面
            menuPanel.SetActive(false);
            
            // 切换摄像机（使用LoginManager中的相同逻辑）
            if (cameraSwitcher != null)
            {
                cameraSwitcher.OnLoginSuccess();
            }
            HideMenuPanel();
            
            // 初始化游戏（根据你的GameManager实现调整）
            if (GameManager.Instance != null)
            {
                GameManager.Instance.InitRace();
            }
            ShowGamePanel();
            Debug.Log("单机模式游戏开始");
        }

        /// <summary>
        /// 点击多人模式按钮的处理
        /// </summary>
        private void OnMultiModeClicked()
        {
            // 隐藏主菜单，显示多人模式界面
            menuPanel.SetActive(false);
            
            if (multiModePanel != null)
            {
                multiModePanel.SetActive(true);
            }
            
            Debug.Log("进入多人模式界面");
        }
        
        private void ShowGamePanel()
        {
            gamePanel.SetActive(true);
        }
        
        private void HideGamePanel()
        {
            gamePanel.SetActive(false);
        }
        private void HideMenuPanel() => menuPanel.SetActive(false);
    }
}