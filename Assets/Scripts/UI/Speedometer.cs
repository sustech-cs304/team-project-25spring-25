using UnityEngine;
using UnityEngine.UI;

public class SpeedNeedleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text speedText;      // 显示速度的文本
    [SerializeField] private Image needleImage;  // 你的指针Image
    
    [Header("Rotation Settings")]
    [SerializeField] private float minSpeed = 0;
    [SerializeField] private float maxSpeed = 200;
    [SerializeField] private float minAngle = -135; // 0速度时的角度
    [SerializeField] private float maxAngle = 135;  // 最大速度时的角度

    void Update()
    {
        // 从速度文本解析当前速度（确保文本只包含数字）
        if(int.TryParse(speedText.text, out int currentSpeed))
        {
            UpdateNeedleRotation(currentSpeed);
        }
    }

    void UpdateNeedleRotation(float speed)
    {
        // 计算旋转角度（线性插值）
        float angle = Mathf.Lerp(minAngle, maxAngle, 
            Mathf.InverseLerp(minSpeed, maxSpeed, speed));
        
        // 应用旋转（UI系统顺时针为正方向）
        needleImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}