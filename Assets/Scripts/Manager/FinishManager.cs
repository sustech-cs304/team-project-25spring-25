using System;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class FinishManager : Singleton<FinishManager>
    {
        [SerializeField] private Image finishBackgroundImage;
        [SerializeField] private Text finishText;
        [SerializeField] private GameObject finishPanel;
        [SerializeField] private Button confirmButton;

        private void Awake()
        {
            confirmButton.onClick.AddListener(Confirm);
        }

        private void Confirm()
        {
            GameManager.Instance.ExitRace();
            HideFinishPanel();
        }

        public void UpdateText(float score)
        {
            finishText.text = $"Congratulations!\n You Finished!\nYour Score: {score:F3}s";
        }

        public void ShowFinishPanel()
        {
            finishPanel.SetActive(true);
        }

        public void HideFinishPanel()
        {
            finishPanel.SetActive(false);
        }
    }
}