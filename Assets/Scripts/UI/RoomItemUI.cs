using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomItemUI : MonoBehaviour
    {
        public TMP_Text roomInfoText;
        public Button joinButton;

        public void SetInfo(string roomInfo)
        {
            roomInfoText.text = roomInfo;
        }

        public void SetJoinCallback(System.Action onClick)
        {
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}