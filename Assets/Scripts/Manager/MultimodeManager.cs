using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultimodeManager : MonoBehaviour, IPointerClickHandler
{
    public GameObject multiPanel; // 设置界面面板
    public Image multiBackgroundImage; // 设置界面的背景Image（需拖拽赋值）

    private void Start()
    {
        // 初始状态关闭
        // if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // 打开设置界面
    // public void OpenSettings()
    // {
    //     if (multiPanel != null) multiPanel.SetActive(true);
    // }

    // 关闭设置界面
    public void CloseMulti()
    {
        if (multiPanel != null) multiPanel.SetActive(false);
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