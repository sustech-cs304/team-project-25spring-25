using System.Collections.Generic;
using Scripts;
using UnityEngine;

namespace Manager
{
    public class CarManager : Singleton<CarManager>
    {
        [SerializeField] private Transform[] playerStartPositions;
        [SerializeField] private Transform[] aiStartPositions;
        [SerializeField] private GameObject carPrefab;
        [SerializeField] private GameObject carsContainer;
        [SerializeField] private GameObject checkpoint;
        [SerializeField] private List<Car> cars;

        public void InitCar(int carNum,int playerNum)
        {
            var checkpoints = new Transform[checkpoint.transform.childCount];
            for (var i = 0; i < checkpoint.transform.childCount; i++)
            {
                checkpoints[i] = checkpoint.transform.GetChild(i).GetComponent<Transform>();
            }
            playerStartPositions = new Transform[playerNum];
            aiStartPositions = new Transform[carNum - playerNum];
            for (var i = 0; i < carNum; i++)
            {
                var car = Instantiate(carPrefab, new Vector3(187 + i * 2, -50, 680 + i * 0.8f), Quaternion.Euler(0, 160, 0), carsContainer.transform);
                if (i < playerNum)
                {
                    playerStartPositions[i] = car.transform;
                    car.GetComponent<Car>().isPlayer = true;
                    car.GetComponent<Car>().EnableCarSpeedUI();
                    car.AddComponent<PlayerController>();
                }
                else
                {
                    aiStartPositions[i-playerNum] = car.transform;
                    car.GetComponent<Car>().isPlayer = false;
                    var carController = car.AddComponent<AiController>();
                    carController.targets = checkpoints;
                }
                cars.Add(car.GetComponent<Car>());
            }
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
            return cars[0].transform;
        }
    }
}