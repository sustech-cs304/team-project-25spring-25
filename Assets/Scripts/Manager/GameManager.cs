
using Manager;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public void InitRace()
        {
            TimeManager.Instance.ResumeGame();
            TimeManager.Instance.CurrentTime = 0;
            CarManager.Instance.InitCar(4,1);
            CameraManager.Instance.SetPlayerCamera(CarManager.Instance.GetCarTransform());
        }
        public void HandleClick()
        {
            if (UIManager.Instance.IsUIOn())
            {
                UIManager.Instance.HandleClick();
            }
        }
    }
}