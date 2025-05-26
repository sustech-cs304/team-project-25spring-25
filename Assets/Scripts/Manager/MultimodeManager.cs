using System;
using System.Collections.Generic;
using System.Net;
using Fusion;
using TMPro;
using UI;
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
        public Button startGameButton;
        public GameObject roomItemPrefab;
        public GameObject roomListUI;
        public GameObject roomInfoUI;
        public Transform contentRoot;
        private List<GameObject> roomItems = new List<GameObject>();
        // 关闭设置界面
        public void CloseMulti()
        {
            if (multiPanel != null) multiPanel.SetActive(false);
        }

        public void UpdateRoomListUI(List<SessionInfo> sessionList,NetworkRunner runner)
        {
            foreach (var item in roomItems)
            {
                Destroy(item);
            }
            roomItems.Clear();
            foreach (var session in sessionList)
            {
                var roomObj = Instantiate(roomItemPrefab, contentRoot);
                var roomUI = roomObj.GetComponent<RoomItemUI>();
                if (roomUI != null)
                {
                    roomUI.SetInfo($"Room: {session.Name}\nPlayers: {session.PlayerCount}/{session.MaxPlayers}");
                    roomUI.SetJoinCallback(() =>
                    {
                        runner.ProvideInput = true;
                        runner.StartGame(new StartGameArgs
                        {
                            GameMode = GameMode.Client,
                            SessionName = session.Name,
                            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
                        });
                        roomListUI.gameObject.SetActive(false);
                        roomInfoUI.gameObject.SetActive(true);
                    });
                }
                roomItems.Add(roomObj);
            }
        }
        public void UpdateRoomUI(string roomInfo)
        {
            var text = roomInfoUI.GetComponentInChildren<TMP_Text>();
            text.text = roomInfo;
        }
        private void Start()
        {
            
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
            startGameButton.onClick.AddListener(OnStartGameClicked);
            startGameButton.gameObject.SetActive(false);
            roomInfoUI.SetActive(false);
        }

        private void OnStartGameClicked()
        {
            NetworkManager.Instance.rpcController.RpcStartRace();
        }
        
        private void OnCreateRoomClicked()
        {
            NetworkManager.Instance.StartHost();
            roomListUI.SetActive(false);
            roomInfoUI.SetActive(true);
            createRoomButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
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
    }
}