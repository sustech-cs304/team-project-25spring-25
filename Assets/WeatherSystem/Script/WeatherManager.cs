using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public enum WeatherType {
    Sunny,
    Rainy
}

public class WeatherManager : MonoBehaviour {
    public static WeatherManager Instance;

    [Header("Weather Volume Profiles")]
    public VolumeProfile sunnyProfile;
    public VolumeProfile rainyProfile;

    [Header("Particle Effects")]
    public GameObject rainParticle;

    [Header("Audio")]
    public AudioSource ambientAudioSource;
    public AudioClip rainSound;

    private Volume volume;
    private WeatherType currentWeather = WeatherType.Sunny;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        volume = FindObjectOfType<Volume>();
        rainParticle.SetActive(false);
    }

    public void SetWeather() {
        StopAllCoroutines();
        if (currentWeather == WeatherType.Sunny) {
            StartCoroutine(TransitionWeather(WeatherType.Rainy));
        } else {
            StartCoroutine(TransitionWeather(WeatherType.Sunny));
        }
        
    }

    IEnumerator TransitionWeather(WeatherType newWeather) {

        switch (newWeather) {
            case WeatherType.Sunny:
                rainParticle.SetActive(false);
                volume.profile = sunnyProfile;
                ambientAudioSource.clip = null; // 停止雨声
                break;
            case WeatherType.Rainy:
                volume.profile = rainyProfile;
                rainParticle.SetActive(true);
                ambientAudioSource.clip = rainSound;
                break;
        }

        currentWeather = newWeather;
        ambientAudioSource.Play();

        yield return null;
    }
}