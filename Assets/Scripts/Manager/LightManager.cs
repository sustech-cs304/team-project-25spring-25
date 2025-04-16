using UnityEngine;

namespace Manager
{
    public class LightManager : Singleton<LightManager>
    {
        [SerializeField] private Material daySkybox;
        [SerializeField] private Material nightSkybox;
        [SerializeField] private Light sunLight;
        [SerializeField] private Light moonLight; 
        [SerializeField] private float dayIntensity; 
        [SerializeField] private float nightIntensity;
        [SerializeField] private float dayDuration ; 
        private void Awake()
        {
            dayIntensity = 1.0f;
            nightIntensity = 0.5f;
            dayDuration = 360f;
        }
        private void Update()
        {
            UpdateDayNightCycle();
        }
        public void SetSunlightIntensity(float intensity)
        {
            if (sunLight != null)
            {
                sunLight.intensity = intensity;
            }
        }
        private void UpdateDayNightCycle()
        {
            var normalizedTime = TimeManager.Instance.CurrentTime % dayDuration / dayDuration;
            var rotationAngle = normalizedTime * 360f - 30f;
            sunLight.transform.rotation = Quaternion.Euler(rotationAngle, 0f, 0f);
            var intensityFactor = Mathf.Clamp01(Mathf.Sin(normalizedTime * Mathf.PI));
            sunLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, intensityFactor);
            moonLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, intensityFactor);
            moonLight.transform.rotation = Quaternion.Euler(rotationAngle + 180f, 0f, 0f);
            if (normalizedTime < 0.5f | normalizedTime>0.8f) // 白天
            {
                RenderSettings.skybox = daySkybox;
            }
            else // 夜晚
            {
                RenderSettings.skybox = nightSkybox;
            }
        }
    }
}