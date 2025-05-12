using Menu;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager
{
    public class GarageManager : Singleton<GarageManager>
    {
        [SerializeField] private GameObject[] cars;
        [SerializeField] private GameObject currentCar;
        [SerializeField] private GameObject map;
        [SerializeField] private GameObject garage;
        [SerializeField] private GameObject garageUI;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button confirmButton;
        
        [SerializeField] private float rotationSpeed = 20f;
        [SerializeField] private int currentIndex;
        [SerializeField] private bool garageMode;

        public void Init()
        {
            CameraManager.Instance.OnGarage();
            map.SetActive(false);
            garage.SetActive(true);
            MenuManager.Instance.HideMenuPanel();
            garageUI.SetActive(true);
            garageMode = true;
            ShowVehicle(currentIndex);
        }

        public void Exit()
        {
            if (currentCar != null)
            {
                Destroy(currentCar);
            }
            CameraManager.Instance.OnLogin();
            garageUI.SetActive(false);
            map.SetActive(true);
            garage.SetActive(false);
            MenuManager.Instance.ShowMenuPanel();
            garageMode = false;
            
        }
        void Start()
        {
            garageMode = false;
            garageUI.SetActive(false);
            leftButton.onClick.AddListener(() => ChangeVehicle(-1));
            rightButton.onClick.AddListener(() => ChangeVehicle(1));
            confirmButton.onClick.AddListener(Confirm);
        }
        void Update()
        {
            if(!garageMode) return;
            currentCar.transform.position = new Vector3(0, 0, 0);
            currentCar.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        void ChangeVehicle(int direction)
        {
            if (currentCar != null)
            {
                Destroy(currentCar);
            }
            currentIndex = (currentIndex + direction + cars.Length) % cars.Length;
            ShowVehicle(currentIndex);
        }

        void ShowVehicle(int index)
        {
            currentCar = Instantiate(cars[index], new Vector3(0,0,0), Quaternion.identity);
            currentCar.transform.localRotation = Quaternion.identity;
        }

        void Confirm()
        {
            CarManager.Instance.CarPrefab = cars[currentIndex];
            Exit();
        }
    }
}