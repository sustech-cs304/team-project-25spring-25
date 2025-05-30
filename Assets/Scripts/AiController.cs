using UnityEngine;

namespace Scripts
{
    public class AiController : MonoBehaviour
    {
        public Car car;
        public Transform[] targets;
        public float turnSensitivity = 10f;
        public float brakingDistance = 1f;
        public float switchTargetDistance = 10f;
        public float maxSpeed = 90f;
        public float maxSteeringAngle = 90f;
        private int currentTargetIndex = 0;
        private void Awake()
        {
            car = GetComponent<Car>();
        }
        private void Update()
        {
            if (targets == null || targets.Length == 0) return;
            var target = targets[currentTargetIndex];
            var toTarget = target.position - transform.position;
            var distance = toTarget.magnitude;

            var directionToTarget = toTarget.normalized;
            var angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
            car.SetSteeringAngle(angleToTarget / maxSteeringAngle);
            // var steering = Mathf.Clamp(angleToTarget / maxSteeringAngle, -1f, 1f);

            int nextTargetIndex = (currentTargetIndex + 1) % targets.Length;
            var nextTarget = targets[nextTargetIndex];
            var toNextTarget = nextTarget.position - target.position;
            var directionToNextTarget = toNextTarget.normalized;

            float turnAngle = Vector3.Angle(directionToTarget, directionToNextTarget);
            Debug.Log(turnAngle);
            bool isCorner = turnAngle > 60f;
            if (distance <= switchTargetDistance) currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
            // if (steering < -0.4f) car.TurnLeft();
            // else if (steering > 0.4f) car.TurnRight();
            // else if (car.steeringAxis != 0f) car.ResetSteeringAngle();
            if (isCorner)
            {
                maxSpeed = 30f;
            }
            else
            {
                maxSpeed = 90f;
            }
            if (car.carSpeed >= maxSpeed)
            {
                car.GoReverse();
                Debug.Log("ai减速");
            }
            else
            {
                if (distance > brakingDistance)
                {
                    car.GoForward();
                    Debug.Log("ai加速");
                }
            }
            car.UpdateData();
        }
    }
}