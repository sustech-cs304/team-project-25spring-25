using Scripts;

namespace PROMETEO___Car_Controller.Scripts.Manager
{
    using System.Collections.Generic;
    using UnityEngine;

    namespace PROMETEO___Car_Controller.Scripts.Manager
    {
        public class CarManager : Singleton<CarManager>
        {
            [SerializeField] private Transform[] playerStartPositions;
            [SerializeField] private Transform[] aiStartPositions;
            [SerializeField] private GameObject carPrefab;
            [SerializeField] private GameObject carsContainer;
            [SerializeField] private List<Car> cars;
            public void InitCar(int carNum,int playerNum)
            {
                playerStartPositions = new Transform[playerNum];
                aiStartPositions = new Transform[carNum - playerNum];
                for (var i = 0; i < carNum; i++)
                {
                    var car = Instantiate(carPrefab, new Vector3(i * 3, 0, 0), Quaternion.identity, carsContainer.transform);
                    if (i < playerNum)
                    {
                        playerStartPositions[i] = car.transform;
                        car.GetComponent<Car>().isPlayer = true;
                        car.AddComponent<PlayerController>();
                    }
                    else
                    {
                        aiStartPositions[i-playerNum] = car.transform;
                        car.GetComponent<Car>().isPlayer = false;
                        car.AddComponent<AiController>();
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
        }
    }
}