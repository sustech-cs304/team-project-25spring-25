using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
      [Range(20, 190)] public int maxSpeed = 90;
      [Range(10, 120)] public int maxReverseSpeed = 45; 
      [Range(1, 10)] public int accelerationMultiplier = 2; 
      [Range(10, 45)] public int maxSteeringAngle = 27; 
      [Range(0.01f, 100f)] public float steeringSpeed = 10f; 
      [Range(100, 1000)] public int brakeForce = 350; 
      [Range(0, 1)] public float rollingFrictionCoefficient = 0.01f; 
      [Range(0, 10)] public float airDragCoefficient = 5f; 
      [Space(10)] public Vector3 bodyMassCenter; 
      //WHEELS
      public GameObject frontLeftMesh;
      public WheelCollider frontLeftCollider;
      public GameObject frontRightMesh;
      public WheelCollider frontRightCollider;
      public GameObject rearLeftMesh;
      public WheelCollider rearLeftCollider;
      public GameObject rearRightMesh;
      public WheelCollider rearRightCollider;
      

      [Space(10)]
      public bool useUI;
      public TrailRenderer RLWTireSkid;
      public TrailRenderer RRWTireSkid;
      public List<ParticleSystem> NitroParticleSystem;
      
      [Space(10)]
      public bool useSounds;
      public AudioSource carEngineSound;
      public AudioSource tireScreechSound;
      float initialCarEngineSoundPitch;
      
      [Space(10)]
      public float nitroCapacity = 100f;
      public float nitroConsumptionRate = 10f;
      public float nitroDriftingRechargeRate = 5f;
      public float nitroAutoRechargeRate = 5f;
      public float nitroBoostMultiplier = 2f;
      
      public float carSpeed;
      
      public Rigidbody carRigidbody;
      public float steeringAxis;
      private float throttleAxis; 
      private float localVelocityZ;
      private float localVelocityX;
      
      private WheelFrictionCurve FLwheelFriction;
      private float FLWextremumSlip;
      private WheelFrictionCurve FRwheelFriction;
      private float FRWextremumSlip;
      private WheelFrictionCurve RLwheelFriction;
      private float RLWextremumSlip;
      private WheelFrictionCurve RRwheelFriction;
      private float RRWextremumSlip;
      public float currentNitro;
      public bool isNitroActive;
      public bool isNitroEffectPlay;
      public bool isDrifting;
      public bool isPlayer;
      public Vector3 resetPos;
      public Quaternion resetRot;

      private void Awake()
    {
        useUI = false;
        currentNitro = 0;
        currentNitro = nitroCapacity;
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
        InitializeWheelFriction(frontLeftCollider, ref FLwheelFriction, ref FLWextremumSlip);
        InitializeWheelFriction(frontRightCollider, ref FRwheelFriction, ref FRWextremumSlip);
        InitializeWheelFriction(rearLeftCollider, ref RLwheelFriction, ref RLWextremumSlip);
        InitializeWheelFriction(rearRightCollider, ref RRwheelFriction, ref RRWextremumSlip);
        
        if(carEngineSound != null) initialCarEngineSoundPitch = carEngineSound.pitch;
        
        if(useSounds){
          InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }else{
          if(carEngineSound != null) carEngineSound.Stop();
          if(tireScreechSound != null) tireScreechSound.Stop();
        }
        if(RLWTireSkid != null) RLWTireSkid.emitting = false;
        if(RRWTireSkid != null) RRWTireSkid.emitting = false;
        foreach (var nitroParticle in NitroParticleSystem)
        {
          nitroParticle.Stop();
        }
    }
    private void InitializeWheelFriction(WheelCollider collider, ref WheelFrictionCurve wheelFriction, ref float extremumSlip)
    {
      wheelFriction = new WheelFrictionCurve
      {
        extremumSlip = collider.sidewaysFriction.extremumSlip,
        extremumValue = collider.sidewaysFriction.extremumValue,
        asymptoteSlip = collider.sidewaysFriction.asymptoteSlip,
        asymptoteValue = collider.sidewaysFriction.asymptoteValue,
        stiffness = collider.sidewaysFriction.stiffness
      };
      extremumSlip = collider.sidewaysFriction.extremumSlip;
    }
    public void UpdateData()
    {
      carSpeed = carRigidbody.velocity.magnitude * 3.6f;
      localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
      localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;
      AnimateWheelMeshes();
      if (useUI) {
        UIManager.Instance.SetPlayerSpeedText(Mathf.RoundToInt(Mathf.Abs(carSpeed)));
        // UIManager.Instance.SetPlayerNitro(currentNitro/nitroCapacity);
      }
      // ApplyRollingFriction();
      // UpdateNitro();
    }

    public void UpdateNitro()
    {
      if (isNitroActive && currentNitro > 0)
      {
        currentNitro -= nitroConsumptionRate * Time.deltaTime;
      }

      if (currentNitro < nitroCapacity) {
        // currentNitro += nitroAutoRechargeRate * Time.deltaTime;
        currentNitro += isDrifting? nitroDriftingRechargeRate * Time.deltaTime : 0;
      }
    }
    public void Update()
    {
      if (useUI) {
          UIManager.Instance.SetPlayerSpeedText(Mathf.RoundToInt(Mathf.Abs(carSpeed)));
          UIManager.Instance.SetPlayerNitro(currentNitro/nitroCapacity);
      }
      ApplyRollingFriction();
      UpdateNitro();
    }

    private void ApplyRollingFriction()
    {
      var velocity = carRigidbody.velocity;
      var frictionForce = rollingFrictionCoefficient * carRigidbody.mass;
      var dragForce = airDragCoefficient * velocity.magnitude * velocity.magnitude;
      if (velocity.magnitude > 0.01f)
        SetBrakeTorque(frictionForce/4 + (isDrifting? brakeForce: 0 * 1.6f) + dragForce);
      else
        carRigidbody.velocity = Vector3.zero;
    }
  
    public void CarSounds(){
      if(useSounds){
          if(carEngineSound != null){
            var engineSoundPitch = initialCarEngineSoundPitch + Mathf.Abs(carRigidbody.velocity.magnitude) / 25f;
            carEngineSound.pitch = engineSoundPitch;
          }
          if(isDrifting || Mathf.Abs(carSpeed) > 12f){
            if(!tireScreechSound.isPlaying) tireScreechSound.Play();
          }else if(!isDrifting && Mathf.Abs(carSpeed) < 12f) tireScreechSound.Stop();
      }else{
        if(carEngineSound != null && carEngineSound.isPlaying) carEngineSound.Stop();
        if(tireScreechSound != null && tireScreechSound.isPlaying) tireScreechSound.Stop();
      }
    }
    
    public void SetSteeringAngle(float targetSteeringAxis) {
      steeringAxis = Mathf.Clamp(targetSteeringAxis, -1f, 1f);
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = steeringAngle;
      frontRightCollider.steerAngle = steeringAngle;
    }

    public void TurnLeft() {
      var newAxis = steeringAxis - Time.deltaTime * steeringSpeed;
      SetSteeringAngle(newAxis);
    }

    public void TurnRight() {
      var newAxis = steeringAxis + Time.deltaTime * steeringSpeed;
      SetSteeringAngle(newAxis);
    }

    public void ResetSteeringAngle() {
      var newAxis = Mathf.MoveTowards(steeringAxis, 0f, Time.deltaTime * steeringSpeed);
      SetSteeringAngle(newAxis);
    }
    private void AnimateWheelMeshes(){
      SetWheelMesh(frontLeftCollider, frontLeftMesh.transform);
      SetWheelMesh(frontRightCollider, frontRightMesh.transform);
      SetWheelMesh(rearLeftCollider, rearLeftMesh.transform);
      SetWheelMesh(rearRightCollider, rearRightMesh.transform);
      return;
      void SetWheelMesh(WheelCollider collider, Transform mesh){
        collider.GetWorldPose(out var position, out var rotation);
        mesh.position = position;
        mesh.rotation = rotation;
      }
    }
    
    public void GoForward(){
      throttleAxis += Time.deltaTime * 3f;
      if(throttleAxis > 1f) throttleAxis = 1f;
      if(localVelocityZ < -1f){
        Brakes();
      }else{
        if(Mathf.RoundToInt(carSpeed) < (isNitroActive&&currentNitro>0 ? 10000f : maxSpeed)&&!isDrifting){
          SetBrakeTorque(0);
          SetMotorTorque(accelerationMultiplier * 50f * (isNitroActive&&currentNitro>0 ? nitroBoostMultiplier : 1) * throttleAxis);
        }else {
          SetMotorTorque(0);
    		}
      }
    }
    public void GoReverse(){
      throttleAxis -= Time.deltaTime * 3f;
      if(throttleAxis < -1f) throttleAxis = -1f;
      if(localVelocityZ > 1f){
        Brakes();
      }else{
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
          SetBrakeTorque(0);
          SetMotorTorque(accelerationMultiplier * 50f * throttleAxis);
        }else {
          SetMotorTorque(0);
    		}
      }
    }
    public void ThrottleOff(){
      SetMotorTorque(0);
    }
    private void Brakes(){
      SetBrakeTorque(brakeForce);
    }
    private void SetMotorTorque(float value) {
      frontLeftCollider.motorTorque = value;
      frontRightCollider.motorTorque = value;
      rearLeftCollider.motorTorque = value;
      rearRightCollider.motorTorque = value;
    }
    private void SetBrakeTorque(float value) {
      frontLeftCollider.brakeTorque = value;
      frontRightCollider.brakeTorque = value;
      rearLeftCollider.brakeTorque = value;
      rearRightCollider.brakeTorque = value;
    }
    public void TryDrift(bool isDriftKeyPressed)
    {
      isDrifting = isDriftKeyPressed;
      PlayDriftEffects();
    }    
    public void TryNitroActive(bool isNitroActiveKeyPressed)
    {
      isNitroActive = isNitroActiveKeyPressed;
      PlayNitroEffects();
    }
    public void PlayDriftEffects()
    {
      RLWTireSkid.emitting = isDrifting;
      RRWTireSkid.emitting = isDrifting;
    }
    public void PlayNitroEffects()
    {
      if (isNitroActive && currentNitro>0 && !isNitroEffectPlay)
      {
        isNitroEffectPlay = true;
        foreach (var nitroParticle in NitroParticleSystem)
        {
          nitroParticle.Play();
        }
      }
      else if (!isNitroActive && isNitroEffectPlay)
      {
        isNitroEffectPlay = false;
        foreach (var nitroParticle in NitroParticleSystem)
        {
          nitroParticle.Stop();
        }
      }
    }
    public void ResetPhysics()
    {
      carRigidbody.velocity = Vector3.zero;
      carRigidbody.angularVelocity = Vector3.zero;
    }

    public void ResetCar()
    {
      ResetPhysics();
      transform.position = resetPos;
      transform.rotation = resetRot;
    }

    public void SetResetValue(Vector3 pos, Quaternion rot)
    {
      resetPos = pos;
      resetRot = rot;
    }
}