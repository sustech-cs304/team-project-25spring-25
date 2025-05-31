using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class SettingsManager : Singleton<SettingsManager>, IPointerClickHandler
    {
        [Header("数据配置")]
        public SettingData settingsData; // 拖拽赋值你的 ScriptableObject

        [Header("UI 组件")]
        public GameObject settingsPanel; // 设置界面面板
        public Image settingBackgroundImage; // 设置界面的背景 Image
        public Slider sensitivitySlider; // 灵敏度滑动条
        public Slider volumeSlider; // 音量滑动条
        public Button exitButton;
        
        private void Start()
        {
            // 初始化灵敏度滑动条
            if (sensitivitySlider != null && settingsData != null)
            {
                sensitivitySlider.value = settingsData.sensitivity;
                sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            }

            // 初始化音量滑动条
            if (volumeSlider == null || settingsData == null) return;
            volumeSlider.value = settingsData.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            exitButton.onClick.AddListener(OnExitClick);
        }

        private void OnExitClick()
        {
            GameManager.Instance.ExitRace();
        }

        // 灵敏度变化回调
        private void OnSensitivityChanged(float value)
        {
            if (settingsData != null)
            {
                settingsData.sensitivity = value;
                Debug.Log($"灵敏度已更新: {value}"); // 可选：调试日志
            }
        }

        // 音量变化回调
        private void OnVolumeChanged(float value)
        {
            if (settingsData != null)
            {
                settingsData.volume = value;
                AudioListener.volume = value; // 直接控制全局音量（可选）
                Debug.Log($"音量已更新: {value}"); // 可选：调试日志
            }
        }

        // 打开设置界面
        public void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        // 关闭设置界面
        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void HideExitButton()
        {
            exitButton.gameObject.SetActive(false);
        }
        public void ShowExitButton()
        {
            exitButton.gameObject.SetActive(true);
        }
        // 检测全局点击事件（点击空白处关闭面板）
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                settingBackgroundImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera))
            {
                CloseSettings();
            }
        }
    }
}