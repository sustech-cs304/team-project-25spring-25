using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager:Singleton<UIManager>
    {
        [SerializeField] private Text playerSpeedText;
        [SerializeField] private Text timeText;
        public void SetPlayerSpeedText(int speed)
        {
            playerSpeedText.text = speed.ToString();
        }
        public void SetTimeText(float time)
        {
            timeText.text = time.ToString("F3") + "s";
        }
        public void Update()
        {
            SetTimeText(TimeManager.Instance.CurrentTime);
        }
    }
}