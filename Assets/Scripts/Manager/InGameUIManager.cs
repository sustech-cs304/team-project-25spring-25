using UnityEngine;
using UnityEngine.UI;


//管理局内设置界面
namespace Manager
{
    public class InGameUIManager : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private GameObject settingsPanel; // 拖拽你的设置面板到这里

        [SerializeField] private Button settingsButton; // 拖拽你的设置按钮到这里

        void Start()
        {
            // 默认隐藏设置界面
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }

            // 绑定按钮点击事件
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(ToggleSettingsPanel);
            }
        }

        // 切换设置面板的显示/隐藏
        public void ToggleSettingsPanel()
        {
            if (settingsPanel != null)
            {
                bool isActive = settingsPanel.activeSelf;
                settingsPanel.SetActive(!isActive);
            }
        }

        // 关闭设置面板（可由其他按钮调用）
        public void CloseSettingsPanel()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }
    }
}