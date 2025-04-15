using System;

namespace Scripts
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public Car car;

        private void Awake()
        {
          car = GetComponent<Car>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && car.currentNitro > 0 && Input.GetKey(KeyCode.W)){
              car.isNitroActive = true;
              car.currentNitro -= car.nitroConsumptionRate * Time.deltaTime;
              if (car.currentNitro < 0)
              {
                car.currentNitro = 0;
                car.isNitroActive = false;
              }
            }
            else
            {
              car.isNitroActive = false;
              if (car.currentNitro < car.nitroCapacity)
              {
                car.currentNitro += car.nitroRechargeRate * Time.deltaTime;
                if (car.currentNitro > car.nitroCapacity)
                {
                  car.currentNitro = car.nitroCapacity;
                }
              }
            }
            if (Input.GetKey(KeyCode.W)){
              CancelInvoke(nameof(car.DecelerateCar));
              car.deceleratingCar = false;
              car.GoForward();
            }
            if(Input.GetKey(KeyCode.S)){
              CancelInvoke(nameof(car.DecelerateCar));
              car.deceleratingCar = false;
              car.GoReverse();
            }

            if(Input.GetKey(KeyCode.A)){
              car.TurnLeft();
            }
            if(Input.GetKey(KeyCode.D)){
              car.TurnRight();
            }
            if(Input.GetKey(KeyCode.Space)){
              CancelInvoke(nameof(car.DecelerateCar));
              car.deceleratingCar = false;
              car.Handbrake();
            }
            if(Input.GetKeyUp(KeyCode.Space)){
              car.RecoverTraction();
            }
            if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
              car.ThrottleOff();
            }
            if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !car.deceleratingCar){
              InvokeRepeating(nameof(car.DecelerateCar), 0f, 0.1f);
              car.deceleratingCar = true;
            }
            if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && car.steeringAxis != 0f){
              car.ResetSteeringAngle();
            }
        }
    }
}