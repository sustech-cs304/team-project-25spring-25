using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Scripts;
using UnityEngine;

namespace Manager
{
    public class CarManager : Singleton<CarManager>
    {
        public GameObject carPrefab;               // 本地单机Prefab
        public NetworkObject networkCarPrefab;     // 联网车Prefab，需带NetworkObject组件
        public NetworkObject carSyncPrefab;        // 联网车同步Prefab，需带NetworkObject组件
        public Transform checkpointParent;
        public Transform carsContainer;
        private List<Car> cars = new();
        private Car playerCar;
        private Transform[] playerStartPositions;
        private Transform[] aiStartPositions;
        private CarSync carSync;
        public void InitCarSinglePlayer(int carNum, int playerNum)
        {
            var checkpoints = new Transform[checkpointParent.childCount];
            for (var i = 0; i < checkpointParent.childCount; i++)
                checkpoints[i] = checkpointParent.GetChild(i);
            playerStartPositions = new Transform[playerNum];
            aiStartPositions = new Transform[carNum - playerNum];
            for (var i = 0; i < carNum; i++)
            {
                var spawnPos = new Vector3(187 + i * 2, -50, 680 + i * 0.8f);
                var spawnRot = Quaternion.Euler(0, 160, 0);
                var carObj = Instantiate(carPrefab, spawnPos, spawnRot, carsContainer);
                var car = carObj.GetComponent<Car>();
                if (i < playerNum) {
                    playerStartPositions[i] = carObj.transform;
                    car.isPlayer = true;
                    carObj.AddComponent<PlayerController>();  // 本地控制
                    car.useUI = true;
                    if (MiniMapSystem.Instance == null) {
                        Debug.LogError("小地图系统未初始化！");
                        return;
                    }
                    // 设置小地图玩家车辆
                    if (MiniMapSystem.Instance != null)
                    {
                        MiniMapSystem.Instance.SetPlayerCar(carObj.transform);
                        Debug.LogError($"MiniMapSystem已经初始化了");
                    }
                }
                else {
                    aiStartPositions[i - playerNum] = carObj.transform;
                    car.isPlayer = false;
                    var aiCtrl = carObj.AddComponent<AiController>();
                    aiCtrl.targets = checkpoints;
                    
                    // 添加敌人标记
                    if (MiniMapSystem.Instance != null)
                        MiniMapSystem.Instance.AddEnemyCar(car);
                }
                cars.Add(car);
            }
            playerCar = cars[0];
        }
        public void InitCarNetwork(List<PlayerRef> playerRefs)
        {
            if (!NetworkManager.Instance.Runner.IsServer)
            {
                while (!carSync)
                {
                    var carSync = FindObjectOfType<CarSync>();
                    this.carSync = carSync;
                }
                return;
            }
            // Host 负责生成所有车辆
            for (var i = 0; i < playerRefs.Count; i++)
            {
                var spawnPos = new Vector3(187 + i * 2, -50, 680 + i * 0.8f);
                var spawnRot = Quaternion.Euler(0, 160, 0);
                var authority = playerRefs[i];
                var netObj = NetworkManager.Instance.Runner.Spawn(networkCarPrefab, spawnPos, spawnRot, authority);
                var car = netObj.GetComponent<Car>();
                car.isPlayer = true;
                netObj.transform.SetParent(carsContainer, false);
                if (netObj.HasInputAuthority)
                {
                    // 设置小地图玩家车辆
                    if (MiniMapSystem.Instance != null)
                        MiniMapSystem.Instance.SetPlayerCar(netObj.transform);
                }
                else
                {
                    // 添加敌人标记
                    if (MiniMapSystem.Instance != null)
                        MiniMapSystem.Instance.AddEnemyCar(car);
                }
            }
            var carSyncObject = NetworkManager.Instance.Runner.Spawn(carSyncPrefab);
            carSync = carSyncObject.GetComponent<CarSync>();
        }
        public void ResetAllCarsToStart()
        {
            var playerIndex = 0;
            var aiIndex = 0;
            foreach (var car in cars)
            {
                if (car.isPlayer)
                {
                    if (playerIndex >= playerStartPositions.Length) continue;
                    ResetCarToPosition(car, playerStartPositions[playerIndex]);
                    playerIndex++;
                }
                else
                {
                    if (aiIndex >= aiStartPositions.Length) continue;
                    ResetCarToPosition(car, aiStartPositions[aiIndex]);
                    aiIndex++;
                }
            }
        }

        private void ResetCarToPosition(Car car, Transform targetTransform)
        {
            car.transform.position = targetTransform.position;
            car.transform.rotation = targetTransform.rotation;
        }

        public Transform GetCarTransform()
        {
            if (playerCar) return playerCar.transform;
            Debug.LogWarning("MyCar is not assigned yet!");
            return null;
        }

        public void RegisterCar(Car car)
        {
            var netObj = car.gameObject.GetComponent<NetworkObject>();
            Debug.Log($"Car registered, HasInputAuthority: {netObj.HasInputAuthority}, Owner: {netObj.InputAuthority}");
            if (!netObj) return;
            if (netObj.HasInputAuthority)
            {
                playerCar = car;
                car.useUI = true;
            }
            cars.Add(car);
        }
        public GameObject CarPrefab
        {
            get => carPrefab;
            set => carPrefab = value;
        }

        public List<Car> Cars
        {
            get => cars;
            set => cars = value;
        }

        public CarSync CarSync
        {
            get => carSync;
            set => carSync = value;
        }

        public void RemoveCars()
        {
            if (MiniMapSystem.Instance != null)
                MiniMapSystem.Instance.ClearAllMarkers();
        
            foreach (var car in cars.ToList())
            {
                cars.Remove(car);
                Destroy(car.gameObject);
            }
        }
    }   
}