using UnityEngine;

namespace Scripts
{
    public class AiController : MonoBehaviour
    {
        public Car car;
        public Transform[] targets;
        public float turnSensitivity = 1.5f;
        public float brakingDistance = 5f;
        public float switchTargetDistance = 2f;
        private int currentTargetIndex = 0;
        private void Update()
        {
            if (targets == null || targets.Length == 0) return;
            var target = targets[currentTargetIndex];
            var toTarget = target.position - transform.position;
            var distance = toTarget.magnitude;
            
            var localTarget = transform.InverseTransformPoint(target.position);
            var steering = Mathf.Clamp(localTarget.x / turnSensitivity, -1f, 1f);
            if (distance <= switchTargetDistance)
            {
                currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
                target = targets[currentTargetIndex];
                toTarget = target.position - transform.position;
                distance = toTarget.magnitude;
            }
            if (steering < -0.1f) car.TurnLeft();
            else if (steering > 0.1f) car.TurnRight();
            else if (car.steeringAxis != 0f) car.ResetSteeringAngle();
            
            if (distance > brakingDistance) {
                CancelInvoke(nameof(car.DecelerateCar));
                car.deceleratingCar = false;
                car.GoForward();
            } else {
                CancelInvoke(nameof(car.DecelerateCar));
                car.deceleratingCar = false;
                car.GoReverse();
            }
            
            if (distance <= brakingDistance && !car.deceleratingCar) {
                InvokeRepeating(nameof(car.DecelerateCar), 0f, 0.1f);
                car.deceleratingCar = true;
            }
            car.UpdateData();
        }
    }
}