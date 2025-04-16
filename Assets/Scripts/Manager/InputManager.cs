using UnityEngine;
namespace Manager
{
    public class InputManager : Singleton<InputManager>
    {
        private void Update()
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.HandleClick();
            }
        }

        private void ToggleMenu()
        {
            var isActive = UIManager.Instance.GetGameMenuActive();
            if (isActive)
            {
                TimeManager.Instance.ResumeGame();
                UIManager.Instance.HideGameMenu();
            }
            else
            {
                TimeManager.Instance.PauseGame();
                UIManager.Instance.ShowGameMenu();
            }
        }
    }
}