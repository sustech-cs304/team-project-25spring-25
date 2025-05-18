using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class MultimodeManager : Singleton<MultimodeManager>, IPointerClickHandler
    {
        public GameObject multiPanel; // 设置界面面板
        public Image multiBackgroundImage; // 设置界面的背景Image（需拖拽赋值）
        public Button createRoomButton;
        public Button joinRoomButton;
        public Button startGameButton;
        public TMP_InputField  ipInputField;
        public TMP_Text  ipDisplayText;
    
        // 关闭设置界面
        public void CloseMulti()
        {
            if (multiPanel != null) multiPanel.SetActive(false);
        }
        private void Start()
        {
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
            joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
            startGameButton.onClick.AddListener(OnStartGameClicked);
            startGameButton.gameObject.SetActive(false);
            ipInputField.text = "10.25.28.40";
            ipDisplayText.text = ""; // 清空 IP 显示
        }

        private void OnStartGameClicked()
        {
            NetworkManager.Instance.rpcController.RpcStartRace();
        }
        
        private void OnCreateRoomClicked()
        {
            NetworkManager.Instance.StartHost();
            // 显示本机IP地址
            var ip = GetLocalIPAddress();
            ipDisplayText.text = $"Create room successfully!\nIP: {ip}";
            createRoomButton.gameObject.SetActive(false);
            joinRoomButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
        }

        private void OnJoinRoomClicked()
        {
            var ip = ipInputField.text.Trim();
            ipInputField.gameObject.SetActive(true);
            if (string.IsNullOrEmpty(ip))
            {
                ipDisplayText.text = "Please enter a valid IP address!";
                return;
            }
            ipDisplayText.text = "The request has been sent...";
            NetworkManager.Instance.StartClient(ip);
            createRoomButton.gameObject.SetActive(false);
            joinRoomButton.gameObject.SetActive(false);
        }
        // 检测全局点击事件
        public void OnPointerClick(PointerEventData eventData)
        {
            // 检查点击是否在 settingBackgroundImage 上
            bool isClickOnImage = RectTransformUtility.RectangleContainsScreenPoint(
                multiBackgroundImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera
            );

            // 如果点击不在 Image 上，则关闭设置界面
            if (!isClickOnImage)
            {
                CloseMulti();
            }
        }
        private string GetLocalIPAddress()
        {
            var localIP = "未知";
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                    localIP = ip.ToString();
                    break;
                }
            }
            catch
            {
                localIP = "获取失败";
            }
            return localIP;
        }
    }
}