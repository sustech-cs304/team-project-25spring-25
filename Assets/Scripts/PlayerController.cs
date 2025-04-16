using System;
using Manager;

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
          var isAccelerating = Input.GetKey(KeyCode.W);
          var isBraking = Input.GetKey(KeyCode.S);
          var isTurningLeft = Input.GetKey(KeyCode.A);
          var isTurningRight = Input.GetKey(KeyCode.D);
          var isHandbraking = Input.GetKey(KeyCode.Space);
          var isNitroPressed = Input.GetKey(KeyCode.LeftShift);
          if (isNitroPressed && car.currentNitro > 0 && isAccelerating) {
            car.isNitroActive = true;
            car.currentNitro -= car.nitroConsumptionRate * Time.deltaTime;
            if (car.currentNitro < 0f)
            {
              car.currentNitro = 0f;
              car.isNitroActive = false;
            }
          }
          else{
            car.isNitroActive = false;
            if (car.currentNitro < car.nitroCapacity)
            {
              car.currentNitro += car.nitroRechargeRate * Time.deltaTime;
              if (car.currentNitro > car.nitroCapacity) car.currentNitro = car.nitroCapacity;
            }
          }
          if (isAccelerating) {
            CancelInvoke(nameof(car.DecelerateCar));
            car.deceleratingCar = false;
            car.GoForward();
          }
          if (isBraking) {
            CancelInvoke(nameof(car.DecelerateCar));
            car.deceleratingCar = false;
            car.GoReverse();
          }
          if (isTurningLeft) car.TurnLeft();
          if (isTurningRight) car.TurnRight();
          if (isHandbraking) {
            CancelInvoke(nameof(car.DecelerateCar));
            car.deceleratingCar = false;
            car.Handbrake();
          }
          if (Input.GetKeyUp(KeyCode.Space)) car.RecoverTraction();
          if (!isAccelerating && !isBraking) car.ThrottleOff();
          if (!isAccelerating && !isBraking && !isHandbraking && !car.deceleratingCar) {
            // InvokeRepeating(nameof(car.DecelerateCar), 0f, 0.1f);
            car.deceleratingCar = true;
          }
          if (!isTurningLeft && !isTurningRight && car.steeringAxis != 0f) car.ResetSteeringAngle();
          car.UpdateData();
        }
    }
}