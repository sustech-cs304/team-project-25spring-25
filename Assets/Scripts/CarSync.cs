using System.Collections.Generic;
using Fusion;
using Manager;
using UnityEngine;

public class CarSync : NetworkBehaviour
{
    [Networked, Capacity(32), SerializeField]  // 假设最多32辆车
    private NetworkDictionary<NetworkId, CarSyncData> CarStates => default;
    

    public override void FixedUpdateNetwork() { 
        if (!HasStateAuthority) return;
        SyncAllCars();
    }

    private void SyncAllCars()
    {
        foreach (var car in CarManager.Instance.Cars)
        {
            if (!car.TryGetComponent<CarNetworkController>(out var controller)) continue;
            var id = controller.Object.Id;
            var data = new CarSyncData
            {
                Position = car.transform.position,
                Rotation = car.transform.rotation,
                Velocity = car.carRigidbody.velocity,
                AngularVelocity = car.carRigidbody.angularVelocity,
                SteeringAxis = car.steeringAxis,
                IsDrifting = car.isDrifting
            };
            CarStates.Set(id, data);
        }
    }

    public bool TryGetCarSyncData(NetworkId carId, out CarSyncData data)
    {
        return CarStates.TryGet(carId, out data);
    }
}
public struct CarSyncData : INetworkStruct
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity; 
    public Vector3 AngularVelocity;
    public float SteeringAxis;
    public bool IsDrifting;
}