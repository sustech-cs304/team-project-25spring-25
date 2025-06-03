using System;

namespace Scripts
{
    using UnityEngine;

    public class TestPlayerController : MonoBehaviour
    {
        public TestCar car;

        private void Awake()
        {
          car = GetComponent<TestCar>();
        }
        private void Update()
        {
          var isAccelerating = Input.GetKey(KeyCode.W);
          var isBraking = Input.GetKey(KeyCode.S);
          var isTurningLeft = Input.GetKey(KeyCode.A);
          var isTurningRight = Input.GetKey(KeyCode.D);
          var isHandbraking = Input.GetKey(KeyCode.Space);
          var isNitroPressed = Input.GetKey(KeyCode.LeftShift);
          if (isAccelerating) {
            car.GoForward();
          }
          if (isBraking) {
            car.GoReverse();
          }
          if (isTurningLeft) car.TurnLeft();
          if (isTurningRight) car.TurnRight();
          if (!isAccelerating && !isBraking) car.ThrottleOff();
          if (!isTurningLeft && !isTurningRight && car.steeringAxis != 0f) car.ResetSteeringAngle();
          car.TryDrift(isHandbraking);
          car.TryNitroActive(isNitroPressed);
          car.UpdateData();
        }

        public void SetCar()
        {
          car = GetComponent<TestCar>();
        }
    }
}