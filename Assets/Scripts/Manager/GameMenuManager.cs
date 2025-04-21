using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class MenuManager : MonoBehaviour
    {
        public Button singleModeButton;    // 单机模式按钮
        public Button multiModeButton;     // 
        public Button settingsButton;      // 设置按钮
        public Button rankingButton;   // 排行榜按钮
        public GameObject menuPanel;       // 当前菜单面板
        public GameObject multiModePanel;  // 多人模式界面
        public GameObject settingsPanel;   // 设置界面
        public GameObject rankingPanel;// 排行榜界面
        public CameraController cameraSwitcher; // 摄像机切换控制器
        public GameObject gamePanel;

        private void Start()
        {
            // 确保初始状态正确
            menuPanel.SetActive(true);
            if (multiModePanel != null) multiModePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (rankingPanel != null) rankingPanel.SetActive(false);
            
            // 添加按钮事件监听
            singleModeButton.onClick.AddListener(OnSingleModeClicked);
            multiModeButton.onClick.AddListener(OnMultiModeClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            rankingButton.onClick.AddListener(OnRankingClicked);
        }

        private void OnDestroy()
        {
            // 移除事件监听
            singleModeButton.onClick.RemoveListener(OnSingleModeClicked);
            multiModeButton.onClick.RemoveListener(OnMultiModeClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            rankingButton.onClick.RemoveListener(OnRankingClicked);
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
        
        
        private void OnSettingsClicked()
        {
            // 如果 Setting 界面已经开启，则直接关闭它
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                return;
            }

            // 如果 Ranking 界面开启，则关闭它
            if (rankingPanel != null && rankingPanel.activeSelf)
            {
                rankingPanel.SetActive(false);
            }

            // 最后，打开 Setting 界面
            settingsPanel.SetActive(true);
        }

        /// <summary>
        /// 点击排行榜按钮的处理
        /// </summary>
        private void OnRankingClicked()
        {
            // 如果 Ranking 界面已经打开，则直接关闭它
            if (rankingPanel.activeSelf)
            {
                rankingPanel.SetActive(false);
                return;
            }

            // 如果 Setting 界面打开，则关闭它
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
            }

            // 最后，打开 Ranking 界面
            rankingPanel.SetActive(true);
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