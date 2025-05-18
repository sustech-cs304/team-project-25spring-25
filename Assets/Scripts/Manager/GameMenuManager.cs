using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager
{
    public class MenuManager : Singleton<MenuManager>
    {
        public Button singleModeButton;    // 单机模式按钮
        public Button multiModeButton;     // 
        public Button settingsButton;      // 设置按钮
        public Button garageButton;      // 车库按钮
        public Button rankingButton;   // 排行榜按钮
        public GameObject menuPanel;       // 当前菜单面板
        public GameObject multiModePanel;  // 多人模式界面
        public GameObject settingsPanel;   // 设置界面
        public GameObject rankingPanel;// 排行榜界面
        public GameObject gamePanel;

        private void Awake()
        {
            // 确保初始状态正确
            menuPanel.SetActive(true);
            if (multiModePanel != null) multiModePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (rankingPanel != null) rankingPanel.SetActive(false);
            if (gamePanel != null) gamePanel.SetActive(false);
            // 添加按钮事件监听
            singleModeButton.onClick.AddListener(OnSingleModeClicked);
            multiModeButton.onClick.AddListener(OnMultiModeClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            garageButton.onClick.AddListener(OnGarageClicked);
            rankingButton.onClick.AddListener(OnRankingClicked);
        }
        

        // private void OnDestroy()
        // {
        //     // 移除事件监听
        //     garageButton.onClick.RemoveListener(OnGarageClicked);
        //     singleModeButton.onClick.RemoveListener(OnSingleModeClicked);
        //     multiModeButton.onClick.RemoveListener(OnMultiModeClicked);
        //     settingsButton.onClick.RemoveListener(OnSettingsClicked);
        //     rankingButton.onClick.RemoveListener(OnRankingClicked);
        // }

        /// <summary>
        /// 点击单机模式按钮的处理
        /// </summary>
        private void OnSingleModeClicked()
        {
            // 隐藏菜单界面
            menuPanel.SetActive(false);
            // 初始化游戏（根据你的GameManager实现调整）
            if (GameManager.Instance != null)
            {
                GameManager.Instance.InitSingleRace();
            }
            
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
        private void OnGarageClicked()
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
            GarageManager.Instance.Init();
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
            
            rankingPanel.SetActive(true);
        }
        /// <summary>
        /// 点击多人模式按钮的处理
        /// </summary>
        private void OnMultiModeClicked()
        {
            // 隐藏主菜单，显示多人模式界面
            // menuPanel.SetActive(false);
            
            if (multiModePanel != null)
            {
                multiModePanel.SetActive(true);
            }
            
            Debug.Log("进入多人模式界面");
        }
        
        public void ShowGamePanel() => gamePanel.SetActive(true);
        public void HideGamePanel() => gamePanel.SetActive(false);
        public void ShowMenuPanel() => menuPanel.SetActive(true);

        public void HideMenuPanel() => menuPanel.SetActive(false);
    }
}