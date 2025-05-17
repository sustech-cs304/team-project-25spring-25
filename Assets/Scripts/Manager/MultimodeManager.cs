using System.Net;
using Fusion;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultimodeManager : MonoBehaviour, IPointerClickHandler
{
    public GameObject multiPanel; // 设置界面面板
    public Image multiBackgroundImage; // 设置界面的背景Image（需拖拽赋值）
    public Button createRoomButton;
    public Button joinRoomButton;
    public InputField ipInputField;
    public Text ipDisplayText;
    
    // 关闭设置界面
    public void CloseMulti()
    {
        if (multiPanel != null) multiPanel.SetActive(false);
    }
    private void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        ipDisplayText.text = ""; // 清空 IP 显示
    }

    private void OnCreateRoomClicked()
    {
        NetworkManager.Instance.StartHost();
        // 显示本机IP地址
        var ip = GetLocalIPAddress();
        ipDisplayText.text = $"房间创建成功，IP: {ip}";
    }

    private void OnJoinRoomClicked()
    {
        var ip = ipInputField.text.Trim();
        if (string.IsNullOrEmpty(ip))
        {
            ipDisplayText.text = "请输入有效的IP地址！";
            return;
        }
        NetworkManager.Instance.StartClient(ip);
        ipDisplayText.text = "加入房间请求已发送...";
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