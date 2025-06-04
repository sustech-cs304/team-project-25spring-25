
using Fusion;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager> 
    {
        public void InitSingleRace()
        {
            CameraManager.Instance.OnGameStart();
            TimeManager.Instance.ResumeGame();
            TimeManager.Instance.CurrentTime = 0;
            MenuManager.Instance.ShowGamePanel();
            MenuManager.Instance.HideMenuPanel();
            SettingsManager.Instance.ShowExitButton();
            CarManager.Instance.InitCarSinglePlayer(2,1);
            CameraManager.Instance.SetPlayerCamera(CarManager.Instance.GetCarTransform());
        }
        public void InitNetworkRace()
        {
            CameraManager.Instance.OnGameStart();
            TimeManager.Instance.ResumeGame();
            TimeManager.Instance.CurrentTime = 0;
            MenuManager.Instance.ShowGamePanel();
            MenuManager.Instance.HideMenuPanel();
            SettingsManager.Instance.ShowExitButton();
            CarManager.Instance.InitCarNetwork(NetworkManager.Instance.connectedPlayers);
            CameraManager.Instance.SetPlayerCamera(CarManager.Instance.GetCarTransform());
        }
        public void HandleClick()
        {
            if (UIManager.Instance.IsUIOn())
            {
                UIManager.Instance.HandleClick();
            }
        }

        public void ExitRace()
        {
            MenuManager.Instance.HideGamePanel();
            MenuManager.Instance.ShowMenuPanel();
            CarManager.Instance.RemoveCars();
            CameraManager.Instance.OnGameExit();
            SettingsManager.Instance.HideExitButton();
        }
    }
}