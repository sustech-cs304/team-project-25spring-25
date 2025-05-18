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
      [Range(0.01f, 1f)] public float steeringSpeed = 0.5f; 
      [Range(100, 600)] public int brakeForce = 350; 
      [Range(1, 10)] public int decelerationMultiplier = 2; 
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
      
      public Rigidbody carRigidbody;
      public float steeringAxis;
      private float throttleAxis; 
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
      public bool isDrifting;
      public bool isPlayer;
    private void Awake()
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
          if(isDrifting || Mathf.Abs(carSpeed) > 12f){
            if(!tireScreechSound.isPlaying) tireScreechSound.Play();
          }else if(!isDrifting && Mathf.Abs(carSpeed) < 12f) tireScreechSound.Stop();
      }else{
        if(carEngineSound != null && carEngineSound.isPlaying) carEngineSound.Stop();
        if(tireScreechSound != null && tireScreechSound.isPlaying) tireScreechSound.Stop();
      }
    }
    
    private void SetSteeringAngle(float targetSteeringAxis) {
      steeringAxis = Mathf.Clamp(targetSteeringAxis, -1f, 1f);
      float steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = steeringAngle;
      frontRightCollider.steerAngle = steeringAngle;
    }

    public void TurnLeft() {
      float newAxis = steeringAxis - Time.deltaTime * steeringSpeed;
      SetSteeringAngle(newAxis);
    }

    public void TurnRight() {
      float newAxis = steeringAxis + Time.deltaTime * steeringSpeed;
      SetSteeringAngle(newAxis);
    }

    public void ResetSteeringAngle() {
      float newAxis = Mathf.MoveTowards(steeringAxis, 0f, Time.deltaTime * steeringSpeed);
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
        if(Mathf.RoundToInt(carSpeed) < (isNitroActive ? 10000f : maxSpeed)){
          SetBrakeTorque(0);
          SetMotorTorque(accelerationMultiplier * 50f * (isNitroActive ? 2 : 1) * throttleAxis);
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
    public void DecelerateCar(){
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
      if (isDriftKeyPressed) {
        EnterDrift();
      }
      else {
        ExitDrift();
      }
    }
    private void EnterDrift()
    {
      isDrifting = Mathf.Abs(localVelocityX) > 2f + 0.1f * carSpeed;
      SetBrakeTorque(brakeForce * 0.8f);
      PlayDriftEffects();
    }
    private void ExitDrift()
    {
      isDrifting = false;
      SetBrakeTorque(0f); 
      StopDriftEffects();
    }
    public void PlayDriftEffects()
    {
      if (!useEffects) return;
      RLWTireSkid.emitting = isDrifting;
      RRWTireSkid.emitting = isDrifting;
    }
    
    public void StopDriftEffects()
    {
      if (!useEffects) return;
      RLWTireSkid.emitting = false;
      RRWTireSkid.emitting = false;
    }
    public void ResetPhysics()
    {
      var rb = GetComponent<Rigidbody>();
      if (rb == null) return;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
    }
}