using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager:Singleton<UIManager>
    {
        [SerializeField] private Text playerSpeedText;

        public void SetPlayerSpeedText(int speed)
        {
            playerSpeedText.text = speed.ToString();
        }
    }
}