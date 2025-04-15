using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ClearPasswordInputeField : MonoBehaviour
{
    public TMP_InputField inputField; // 引用TMP_InputField组件
    public string placeholderText = "Enter your password"; // 提示文本

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onSelect.AddListener(OnInputFieldSelected);
            inputField.onDeselect.AddListener(OnInputFieldDeselected);
        }
    }

    private void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSelect.RemoveListener(OnInputFieldSelected);
            inputField.onDeselect.RemoveListener(OnInputFieldDeselected);
        }
    }

    private void OnInputFieldSelected(string text)
    {
        // 当输入字段被选中时，清空文本
        inputField.text = "";
    }

    private void OnInputFieldDeselected(string text)
    {
        // 当输入字段失去焦点时，显示提示文本
        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = placeholderText;
        }
    }
}
