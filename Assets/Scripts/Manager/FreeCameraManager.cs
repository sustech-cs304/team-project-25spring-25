using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreelookCameraManager : MonoBehaviour
{
    [Header("Settings Reference")]
    [SerializeField] private SettingData settingsData;
    
    [Header("Sensitivity Configuration")]
    [Tooltip("Base multiplier for X axis rotation speed")]
    [SerializeField] private float xAxisSensitivityMultiplier = 6f;
    [Tooltip("Y axis speed will be X axis speed divided by this value")]
    [SerializeField] private float yAxisDivider = 150f;
    [Tooltip("Smoothness for acceleration/deceleration (seconds)")]
    [SerializeField] private float accelTime = 0.2f;
    [SerializeField] private float decelTime = 0.1f;

    private CinemachineFreeLook freeLookCamera;
    private float lastSensitivityValue;

    private void Awake()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        InitializeCameraSettings();
    }

    private void InitializeCameraSettings()
    {
        if (freeLookCamera == null)
        {
            Debug.LogError("CinemachineFreeLook component not found!", this);
            return;
        }

        if (settingsData == null)
        {
            Debug.LogWarning("SettingData reference not set. Using default values.", this);
            UpdateCameraSensitivity(1f); // Default sensitivity
            return;
        }

        // Initial setup
        UpdateCameraSensitivity(settingsData.sensitivity);
        lastSensitivityValue = settingsData.sensitivity;
    }

    private void Update()
    {
        if (settingsData != null && !Mathf.Approximately(lastSensitivityValue, settingsData.sensitivity))
        {
            UpdateCameraSensitivity(settingsData.sensitivity);
            lastSensitivityValue = settingsData.sensitivity;
        }
    }

    private void UpdateCameraSensitivity(float sensitivity)
    {
        // Calculate speeds
        float xSpeed = sensitivity * xAxisSensitivityMultiplier;
        float ySpeed = xSpeed / yAxisDivider;

        // Apply to X axis (horizontal rotation)
        freeLookCamera.m_XAxis.m_MaxSpeed = xSpeed;
        freeLookCamera.m_XAxis.m_AccelTime = accelTime;
        freeLookCamera.m_XAxis.m_DecelTime = decelTime;

        // Apply to Y axis (vertical rotation)
        freeLookCamera.m_YAxis.m_MaxSpeed = ySpeed;
        freeLookCamera.m_YAxis.m_AccelTime = accelTime;
        freeLookCamera.m_YAxis.m_DecelTime = decelTime;
    }

    // Editor-only validation
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (freeLookCamera == null)
            freeLookCamera = GetComponent<CinemachineFreeLook>();
            
        if (freeLookCamera != null && settingsData != null)
        {
            UpdateCameraSensitivity(settingsData.sensitivity);
        }
    }
    #endif
}