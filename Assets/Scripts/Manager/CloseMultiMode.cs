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
    public class CloseMultiMode : Singleton<CloseMultiMode>, IPointerClickHandler
    {
        public GameObject multiPanel; // 设置界面面板
        public Image multiBackgroundImage; // 设置界面的背景Image（需拖拽赋值）
        // 关闭多人界面
        public void CloseMulti()
        {
            if (multiPanel != null) multiPanel.SetActive(false);
        }
        // 检测全局点击事件
        public void OnPointerClick(PointerEventData eventData)
        {
            // 检查点击是否在 multipanelimage 上
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