using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class LevelManager : Singleton<LevelManager>, IPointerClickHandler
    {
        [SerializeField] private Text levelText;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private int maxLevel;
        [SerializeField] private int level;
        [SerializeField] private Image levelBackgroundImage;
        [SerializeField] private GameObject levelPanel;

        private void Awake()
        {
            maxLevel = 2;
            level = 1;
            levelText.text = "Level " + level;
            leftButton.onClick.AddListener(() => ChangeLevel(-1));
            rightButton.onClick.AddListener(() => ChangeLevel(1));
            confirmButton.onClick.AddListener(Confirm);
        }

        private void Confirm()
        {
            leftButton.transform.parent.gameObject.SetActive(false);
            GameManager.Instance.InitSingleRace(level);
            Debug.Log("单机模式游戏开始");
        }

        private void ChangeLevel(int i)
        {
            level = (level + maxLevel + i)%maxLevel;
            levelText.text = "Level " + (level + 1);
        }
        public void CloseLevel()
        {
            if (levelPanel != null) levelPanel.SetActive(false);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            // 检查点击是否在 rankingBackgroundImage 上
            var isClickOnImage = RectTransformUtility.RectangleContainsScreenPoint(
                levelBackgroundImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera
            );

            // 如果点击不在 Image 上，则关闭设置界面
            if (!isClickOnImage)
            {
                CloseLevel();
            }
        }
    }
}