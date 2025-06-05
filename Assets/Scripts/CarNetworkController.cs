using System;
using Fusion;
using Fusion.Sockets;
using Manager;
using UnityEngine;

public class CarNetworkController : NetworkBehaviour
{
    [SerializeField] private Car car;

    // 网络同步属性（输入状态）
    [Networked] private NetworkBool IsAccelerating { get; set; }
    [Networked] private NetworkBool IsBraking { get; set; }
    [Networked] private NetworkBool IsTurningLeft { get; set; }
    [Networked] private NetworkBool IsTurningRight { get; set; }
    [Networked] private NetworkBool IsHandbraking { get; set; }
    [Networked] private NetworkBool IsNitroPressed { get; set; }

    // 影子状态（客户端本地缓存）
    private class CarGhostState
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public float SteeringAxis;
        public bool IsDrifting;
        public float CurrentNitro;
    }

    private CarGhostState ghostState = new();
    private float syncTimer;
    private const float SyncInterval = 0.02f; // 每 0.02 秒同步一次（50 FPS）

    // 相位滞后距离，单位米，调整追踪距离
    [SerializeField]
    private float phaseLagDistance = 0.5f;

    public override void Spawned()
    {        
        Runner.SetIsSimulated(Object,true);
        car = gameObject.GetComponent<Car>();
        CarManager.Instance.RegisterCar(Car);
    }

    public override void FixedUpdateNetwork()
    {
        if (!car || !car.carRigidbody)
            return;
        // 采集输入
        if (GetInput(out NetworkInputData inputData))
        {
            IsAccelerating = inputData.IsAccelerating;
            IsBraking = inputData.IsBraking;
            IsTurningLeft = inputData.IsTurningLeft;
            IsTurningRight = inputData.IsTurningRight;
            IsHandbraking = inputData.IsHandbraking;
            IsNitroPressed = inputData.IsNitroPressed;
        }
        if (HasStateAuthority)
        {
            SimulateCar();
        }
        else
        {
            // 客户端非控制端：根据服务器同步数据做影子跟随+插值
            var carSync = CarManager.Instance.CarSync;
            if (carSync == null || !carSync.TryGetCarSyncData(Object.Id, out var syncData))
                return;
            syncTimer += Runner.DeltaTime;
            if (syncTimer >= SyncInterval)
            {
                syncTimer = 0f;
                ghostState.Position = syncData.Position;
                ghostState.Rotation = syncData.Rotation;
                ghostState.Velocity = syncData.Velocity;
                ghostState.AngularVelocity = syncData.AngularVelocity;
                ghostState.SteeringAxis = syncData.SteeringAxis;
                ghostState.IsDrifting = syncData.IsDrifting;
                ghostState.CurrentNitro = syncData.CurrentNitro;
            }
            // Debug.Log($"[Ghost Sync] ID: {Object.Id}\n" +
            //           $"Pos: {syncData.Position}, Rot: {syncData.Rotation.eulerAngles}\n" +
            //           $"Vel: {syncData.Velocity}, AVel: {syncData.AngularVelocity}\n" +
            //           $"Steering: {syncData.SteeringAxis}, Drifting: {syncData.IsDrifting}");
            FollowGhost(phaseLagDistance);
        }
    }

    private void FollowGhost(float phaseLag)
    {
        var currentPos = car.transform.position;
        var targetPos = ghostState.Position;

        var direction = targetPos - currentPos;
        var distance = direction.magnitude;

        if (phaseLag > 0f && distance > 0.001f)
        {
            direction.Normalize();
            targetPos -= direction * phaseLag;
        }
        
        // 也可以用一个最大插值速度限制来避免跳跃
        var lerpFactor = Mathf.Clamp(10f * Runner.DeltaTime, 0f, 0.5f);

        var newPos = Vector3.Lerp(currentPos, targetPos, lerpFactor);
        var newRot = Quaternion.Slerp(car.transform.rotation, ghostState.Rotation, lerpFactor);
        Quaternion.Normalize(newRot);
        car.carRigidbody.MovePosition(newPos);
        car.carRigidbody.MoveRotation(newRot);

        car.carRigidbody.velocity = Vector3.Lerp(car.carRigidbody.velocity, ghostState.Velocity, lerpFactor);
        car.carRigidbody.angularVelocity = Vector3.Lerp(car.carRigidbody.angularVelocity, ghostState.AngularVelocity, lerpFactor);
        
        car.steeringAxis = Mathf.Lerp(car.steeringAxis, ghostState.SteeringAxis, lerpFactor);
        car.isDrifting = ghostState.IsDrifting;
        car.currentNitro = ghostState.CurrentNitro;
        car.PlayDriftEffects();
    }

    private void SimulateCar()
    {
        if (IsAccelerating)
        {
            car.GoForward();
        }
        if (IsBraking)
        {
            car.GoReverse();
        }
        if (IsTurningLeft) car.TurnLeft();
        if (IsTurningRight) car.TurnRight();
        if (!IsAccelerating && !IsBraking) car.ThrottleOff();
        if (!IsTurningLeft && !IsTurningRight && car.steeringAxis != 0f)
            car.ResetSteeringAngle();
        car.TryDrift(IsHandbraking);
        car.TryNitroActive(IsNitroPressed);
        car.UpdateData();
    }

    public Car Car
    {
        get => car;
        set => car = value;
    }
}

public struct NetworkInputData : INetworkInput
{
    public bool IsAccelerating;
    public bool IsBraking;
    public bool IsTurningLeft;
    public bool IsTurningRight;
    public bool IsHandbraking;
    public bool IsNitroPressed;
}
