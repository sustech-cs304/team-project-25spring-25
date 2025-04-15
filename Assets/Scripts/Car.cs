using System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
      [Range(20, 190)] public int maxSpeed = 90;
      [Range(10, 120)] public int maxReverseSpeed = 45; 
      [Range(1, 10)] public int accelerationMultiplier = 2; 
      [Range(10, 45)] public int maxSteeringAngle = 27; 
      [Range(0.1f, 1f)] public float steeringSpeed = 0.5f; 
      [Range(100, 600)] public int brakeForce = 350; 
      [Range(1, 10)] public int decelerationMultiplier = 2; 
      [Range(1, 10)] public int handbrakeDriftMultiplier = 5; 
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
      
      public bool useEffects;
      public ParticleSystem RLWParticleSystem;
      public ParticleSystem RRWParticleSystem;
      [Space(10)]

      public TrailRenderer RLWTireSkid;
      public TrailRenderer RRWTireSkid;
      public ParticleSystem NitroParticleSystem;
      
      [Space(10)]
      public bool useSounds;
      public AudioSource carEngineSound;
      public AudioSource tireScreechSound;
      float initialCarEngineSoundPitch;
      
      [Space(10)]
      public float nitroCapacity = 100f;
      public float nitroConsumptionRate = 10f;
      public float nitroRechargeRate = 5f;
      public float nitroBoostMultiplier = 2f;
      
      public float carSpeed;
      public bool isDrifting;
      public bool isTractionLocked; 
      
      private Rigidbody carRigidbody;
      public float steeringAxis;
      private float throttleAxis; 
      private float driftingAxis;
      private float localVelocityZ;
      private float localVelocityX;

      public bool deceleratingCar;
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
      public bool isPlayer;
    private void Start()
    {
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
        if (useEffects) return;
        if(RLWParticleSystem != null) RLWParticleSystem.Stop();
        if(RRWParticleSystem != null) RRWParticleSystem.Stop();
        if(RLWTireSkid != null) RLWTireSkid.emitting = false;
        if(RRWTireSkid != null) RRWTireSkid.emitting = false;
        if(NitroParticleSystem != null) NitroParticleSystem.Stop();
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
    }
    
    public void CarSpeedUI(){
        UIManager.Instance.SetPlayerSpeedText(Mathf.RoundToInt(Mathf.Abs(carSpeed)));
    }
    public void EnableCarSpeedUI(){
      InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
    }
    public void DisableCarSpeedUI(){
      InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
    }
    public void CarSounds(){
      if(useSounds){
          if(carEngineSound != null){
            var engineSoundPitch = initialCarEngineSoundPitch + Mathf.Abs(carRigidbody.velocity.magnitude) / 25f;
            carEngineSound.pitch = engineSoundPitch;
          }
          if(isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)){
            if(!tireScreechSound.isPlaying) tireScreechSound.Play();
          }else if(!isDrifting && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)) tireScreechSound.Stop();
      }else{
        if(carEngineSound != null && carEngineSound.isPlaying) carEngineSound.Stop();
        if(tireScreechSound != null && tireScreechSound.isPlaying) tireScreechSound.Stop();
      }
    }
    
    private void SetSteeringAngle(float targetSteeringAxis){
      steeringAxis = Mathf.Clamp(targetSteeringAxis, -1f, 1f);
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    public void TurnLeft(){
      SetSteeringAngle(steeringAxis - Time.deltaTime * 10f * steeringSpeed);
    }

    public void TurnRight(){
      SetSteeringAngle(steeringAxis + Time.deltaTime * 10f * steeringSpeed);
    }

    public void ResetSteeringAngle(){
      steeringAxis = Mathf.MoveTowards(steeringAxis, 0f, Time.deltaTime * 10f * steeringSpeed);
      if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f) steeringAxis = 0f;
      SetSteeringAngle(steeringAxis);
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
      isDrifting = Mathf.Abs(localVelocityX) > 4f + 0.2 * carSpeed;
      DriftCarPS();
      throttleAxis += Time.deltaTime * 3f;
      if(throttleAxis > 1f) throttleAxis = 1f;
      if(localVelocityZ < -1f){
        Brakes();
      }else{
        if(Mathf.RoundToInt(carSpeed) < (isNitroActive ? 10000f : maxSpeed)){
          SetBrakeTorque(0);
          SetMotorTorque(accelerationMultiplier * 50f * (isNitroActive ? 2 : 1) * throttleAxis);
        }else {
          SetMotorTorque(0);
    		}
      }
    }
    public void GoReverse(){
      isDrifting = Mathf.Abs(localVelocityX) > 4f + 0.2 * carSpeed;
      DriftCarPS();
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
    public void DecelerateCar(){
      isDrifting = Mathf.Abs(localVelocityX) > 4f + 0.2 * carSpeed;
      DriftCarPS();
      throttleAxis = Mathf.MoveTowards(throttleAxis, 0f, Time.deltaTime * 10f);
      if (Mathf.Abs(throttleAxis) < 0.15f) throttleAxis = 0f;
      carRigidbody.velocity *= 1f / (1f + 0.025f * decelerationMultiplier);
      SetMotorTorque(0);
      if (!(carRigidbody.velocity.magnitude < 0.25f)) return;
      carRigidbody.velocity = Vector3.zero;
      CancelInvoke(nameof(DecelerateCar));
    }
    private void Brakes(){
      SetBrakeTorque(brakeForce);
    }
    
    public void Handbrake(){
      CancelInvoke(nameof(RecoverTraction));
      driftingAxis += Time.deltaTime;
      var secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;
      if(secureStartingPoint < FLWextremumSlip){
        driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
      }
      if(driftingAxis > 1f){
        driftingAxis = 1f;
      }
      isDrifting = Mathf.Abs(localVelocityX) > 2f + 0.1 * carSpeed;
      if(driftingAxis < 1f){
        UpdateWheelFriction();
      }
      isTractionLocked = true;
      DriftCarPS();
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
    private void DriftCarPS(){
      if(useEffects){
          if(isDrifting){
            RLWParticleSystem.Play();
            RRWParticleSystem.Play();
          }else{
            RLWParticleSystem.Stop();
            RRWParticleSystem.Stop();
          }
          var isSkid = (isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f;
          RLWTireSkid.emitting = isSkid;
          RRWTireSkid.emitting = isSkid;
      }else{
        if (RLWParticleSystem && RLWParticleSystem.isPlaying) RLWParticleSystem.Stop();
        if (RRWParticleSystem && RRWParticleSystem.isPlaying) RRWParticleSystem.Stop();
        if (RLWTireSkid) RLWTireSkid.emitting = false;
        if (RRWTireSkid) RRWTireSkid.emitting = false;
        if (NitroParticleSystem && NitroParticleSystem.isPlaying) NitroParticleSystem.Stop();
      }
    }
    public void RecoverTraction(){
      isTractionLocked = false;
      driftingAxis = Mathf.Max(0f, driftingAxis - Time.deltaTime / 1.5f); 
      if(FLwheelFriction.extremumSlip > FLWextremumSlip)
      {
        UpdateWheelFriction();
        Invoke(nameof(RecoverTraction), Time.deltaTime);
      }else if (FLwheelFriction.extremumSlip < FLWextremumSlip){
        ResetWheelFriction();
        driftingAxis = 0f;
      }
    }

    private void UpdateWheelFriction()
    {
      FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      frontLeftCollider.sidewaysFriction = FLwheelFriction;
      FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      frontRightCollider.sidewaysFriction = FRwheelFriction;
      RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      rearLeftCollider.sidewaysFriction = RLwheelFriction;
      RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      rearRightCollider.sidewaysFriction = RRwheelFriction;
    }
    private void ResetWheelFriction() {
      FLwheelFriction.extremumSlip = FLWextremumSlip;
      frontLeftCollider.sidewaysFriction = FLwheelFriction;
      FRwheelFriction.extremumSlip = FRWextremumSlip;
      frontRightCollider.sidewaysFriction = FRwheelFriction;
      RLwheelFriction.extremumSlip = RLWextremumSlip;
      rearLeftCollider.sidewaysFriction = RLwheelFriction;
      RRwheelFriction.extremumSlip = RRWextremumSlip;
      rearRightCollider.sidewaysFriction = RRwheelFriction;
    }

    public void ResetPhysics()
    {
      var rb = GetComponent<Rigidbody>();
      if (rb == null) return;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
    }
}