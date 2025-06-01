using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager:Singleton<UIManager>
    {
        [SerializeField] private Text playerSpeedText;
        [SerializeField] private Text timeText;
        [SerializeField] private Image Nitro;
        [SerializeField] private GameObject gameMenu;
        [SerializeField] private List<GameObject> displayUI;
        
        void Awake() {
            var uiRoots = new List<GameObject> {
                playerSpeedText.transform.root.gameObject,
                timeText.transform.root.gameObject,
                Nitro.transform.root.gameObject
            };
    
            foreach(var root in uiRoots) {
                if(root != null) DontDestroyOnLoad(root);
            }
        }
        public void SetPlayerSpeedText(int speed)
        {
            playerSpeedText.text = speed.ToString();
        }
        public void SetTimeText(float time)
        {
            timeText.text = time.ToString("F3") + "s";
        }
        public void SetPlayerNitro(float nitro)
        {
            Nitro.fillAmount = nitro;
        }
        public void ShowGameMenu()
        {
            displayUI.Add(gameMenu);
            gameMenu.SetActive(true);
        }
        public void HideGameMenu()
        {
            gameMenu.SetActive(false);
            displayUI.Remove(gameMenu);
        }
        public bool GetGameMenuActive()
        {
            return gameMenu.activeSelf;
        }
        public void Update()
        {
            SetTimeText(TimeManager.Instance.CurrentTime);
        }
        public void ShowDisplayUI()
        {
            foreach (var ui in displayUI)
            {
                ui.SetActive(true);
            }
        }
        public void HideDisplayUI()
        {
            foreach (var ui in displayUI)
            {
                ui.SetActive(false);
            }
            displayUI.Clear();
        }
        public void HandleClick()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                HideDisplayUI();
            }
        }

        public bool IsUIOn()
        {
            return displayUI.Count != 0;
        }
    }
}