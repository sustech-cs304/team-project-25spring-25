using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;

    public void OnLoginButtonClicked()
    {
        string id = idInput.text;
        string password = passwordInput.text;
        if (id=="123456"&&password=="123456")
        {
            CloseCameraCullingMask("testLayer");
            Camera.main.cullingMask = 1 << 0 | 1 << 5;
        }
    }
    
    private void SetGameObjectLayer(GameObject go, string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
 
        Debug.Log(GetType()+ "/SetGameObjectLayer()/ layer : "+ layer);
 
        SetGameObjectLayer(go, layer);
    }
    
    private void SetGameObjectLayer(GameObject go, int layer) {
        go.layer = layer;
    }
    
    private void SetCameraCullingMask(string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
        SetCameraCullingMask(layer);
    }
 
    /// <summary>
    /// 通过层的序号设置 Camera CullingMask
    /// </summary>
    /// <param name="layer"></param>
    private void SetCameraCullingMask(int layer)
    {
        Camera.main.cullingMask = 1 << layer;
    }
    
    private void CloseCameraCullingMask(string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
        CloseCameraCullingMask(layer);
    }
    
    private void CloseCameraCullingMask(int layer)
    {
        Camera.main.cullingMask = 0 << layer;
    }
}
