using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class RankingManager : Singleton<RankingManager>, IPointerClickHandler
    {
        public GameObject rankingPanel; // 设置界面面板
        public Image rankingBackgroundImage; // 设置界面的背景Image（需拖拽赋值）
        public Text rankingText;
        public Text levelText;
        public Button leftButton;
        public Button rightButton;
        public int level;
        public int maxLevel;
        private void Awake()
        {
            level = 0;
            maxLevel = 3;
            leftButton.onClick.AddListener(() => ChangeLevel(-1));
            rightButton.onClick.AddListener(() => ChangeLevel(1));
        }

        public void UpdateText()
        {
            if (ScoreManager.Instance == null) return;

            var entries = ScoreManager.Instance.ScoreData;

            // 筛选当前 level 的分数
            var filtered = entries
                .Where(e => e.ContainsKey("level") && JsonConvert.DeserializeObject<int>(e["level"].ToString()) == level)
                .OrderBy(e => e.ContainsKey("score") ? JsonConvert.DeserializeObject<float>(e["score"].ToString()) : 0)
                .Take(10)
                .ToList();

            // 构建显示字符串
            var result = "";
            for (var i = 0; i < filtered.Count; i++)
            {
                var entry = filtered[i];

                var username = entry.ContainsKey("username") ? entry["username"].ToString() : "";
                var score = entry.ContainsKey("score") ? JsonConvert.DeserializeObject<float>(entry["score"].ToString()) : 0;

                result += $"Rank.{i + 1} - {username} - {score:F3}s\n";
            }

            rankingText.text = result == "" ? "No data for this level." : result;
            levelText.text = $"Level {level + 1}";
        }

        // 打开设置界面
        public void OpenRanking()
        {
            if (rankingPanel != null) rankingPanel.SetActive(true);
        }
        private void ChangeLevel(int i)
        {
            level = (level + maxLevel + i)%maxLevel;
            levelText.text = "Level " + (level + 1);
            UpdateText();
        }
        // 关闭设置界面
        public void CloseRanking()
        {
            if (rankingPanel != null) rankingPanel.SetActive(false);
        }

        // 检测全局点击事件
        public void OnPointerClick(PointerEventData eventData)
        {
            // 检查点击是否在 rankingBackgroundImage 上
            bool isClickOnImage = RectTransformUtility.RectangleContainsScreenPoint(
                rankingBackgroundImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera
            );

            // 如果点击不在 Image 上，则关闭设置界面
            if (!isClickOnImage)
            {
                CloseRanking();
            }
        }
    }
}