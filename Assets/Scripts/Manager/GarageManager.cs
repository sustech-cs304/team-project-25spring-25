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
        [SerializeField] private GameObject carInfoUI;
        [SerializeField] private Text carInfo;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button confirmButton;
        
        [SerializeField] private float rotationSpeed = 20f;
        [SerializeField] private int currentIndex;
        [SerializeField] private Quaternion carRotation;
        [SerializeField] private bool garageMode;

        public void Init()
        {
            CameraManager.Instance.OnGarage();
            map.SetActive(false);
            garage.SetActive(true);
            MenuManager.Instance.HideMenuPanel();
            garageMode = true;
            carInfoUI.gameObject.SetActive(true);
            carRotation = Quaternion.Euler(0, 0, 0);
            ShowVehicle();
            garageUI.SetActive(true);
        }

        public void Exit()
        {
            if (currentCar != null)
            {
                Destroy(currentCar);
            }
            carInfoUI.gameObject.SetActive(false);
            CameraManager.Instance.OnLogin();
            garageUI.SetActive(false);
            map.SetActive(true);
            garage.SetActive(false);
            MenuManager.Instance.ShowMenuPanel();
            garageMode = false;
            
        }
        void Awake()
        {
            garageMode = false;
            garageUI.SetActive(false);
            carInfoUI.SetActive(false);
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
            ShowVehicle();
        }

        void ShowVehicle()
        {
            
            if(currentCar) carRotation = currentCar.transform.rotation;
            currentCar = Instantiate(cars[currentIndex], new Vector3(0,0,0), Quaternion.identity);
            currentCar.transform.rotation = carRotation;
            UpdateCarInfo();
        }
        private void UpdateCarInfo()
        {
            var car = currentCar.GetComponent<Car>();
            carInfo.text = 
                $"- Name: {car.name.Replace("(Clone)","")}\n" +
                $"- Max Speed: {car.maxSpeed} km/h\n" +
                $"- Acceleration: x{car.accelerationMultiplier}\n" +
                $"- Steering Angle: {car.maxSteeringAngle}°\n" +
                $"- Steering Speed: {car.steeringSpeed:F2}\n" +
                $"- Brake Force: {car.brakeForce} N\n" +
                $"- Nitro Capacity: {car.nitroCapacity:F1}\n" +
                $"- Drifting Recharge: {car.nitroDriftingRechargeRate:F1}\n" +
                $"- Boost Multiplier: {car.nitroBoostMultiplier:F1}\n";
        }

        void Confirm()
        {
            CarManager.Instance.CarPrefab = cars[currentIndex];
            Exit();
        }
    }
}