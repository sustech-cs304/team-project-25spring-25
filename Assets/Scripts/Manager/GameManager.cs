using PROMETEO___Car_Controller.Scripts.Manager.PROMETEO___Car_Controller.Scripts.Manager;
using UnityEngine;

namespace PROMETEO___Car_Controller.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public void InitRace()
        {
            TimeManager.Instance.ResumeGame();
            TimeManager.Instance.CurrentTime = 0;
            CarManager.Instance.InitCar(4,1);
        }
    }
}