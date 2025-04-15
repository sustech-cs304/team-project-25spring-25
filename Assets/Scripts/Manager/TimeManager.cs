using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class TimeManager : Singleton<TimeManager>
    {
        [SerializeField] private float currentTime = 0f; // 游戏当前时间
        [SerializeField] private bool isPaused = true; // 是否暂停游戏时间
        // 实例初始化
        private void Start()
        {
            InitTime();
            PauseGame();
        }
        // 每秒更新时间
        private void Update()
        {
            if (!isPaused)
            {
                // 增加时间，使用 Time.deltaTime 与 timeScale 控制时间流逝
                currentTime += Time.deltaTime;
            }
        }
        // 时间初始化
        public void InitTime()
        {
            currentTime = 0f;
            isPaused = false;
        }
        // 暂停游戏时间
        public void PauseGame()
        {
            isPaused = true;
        }
        public void TogglePause()
        {
            isPaused = !isPaused;
        }
        public void ResumeGame()
        {
            isPaused = false;
        }
        public float CurrentTime
        {
            get => currentTime;
            set => currentTime = value;
        }
        public bool IsPaused
        {
            get => isPaused;
            set => isPaused = value;
        }
    }
}