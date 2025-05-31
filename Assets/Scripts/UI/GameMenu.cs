// using Manager;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace UI
// {
//     public class GameMenu : MonoBehaviour
//     {
//         [SerializeField] private Button backGameButton; 
//         [SerializeField] private Button quitMenuButton; 
//         [SerializeField] private Button quitGameButton; 
//
//         private void Start()
//         {
//             gameObject.SetActive(false);
//             // 给按钮添加点击事件
//             backGameButton.onClick.AddListener(BackGame);
//             quitMenuButton.onClick.AddListener(QuitMenu);
//             quitGameButton.onClick.AddListener(QuitGame);
//         }
//         
//         public void BackGame()
//         {
//             gameObject.SetActive(false);
//             TimeManager.Instance.ResumeGame();
//         }
//         public void QuitMenu()
//         {
//             gameObject.SetActive(false);
//             // UIManager.Instance.ShowStartMenu();
//             // TimeManager.Instance.HideTime();
//         }
//
//         public void QuitGame()
//         {   
//             Application.Quit();
//         }
//     }
// }