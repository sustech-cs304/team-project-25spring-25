/*
MESSAGE FROM CREATOR: This script was coded by Mena. You can use it in your games either these are commercial or
personal projects. You can even add or remove functions as you wish. However, you cannot sell copies of this
script by itself, since it is originally distributed as a free product.
I wish you the best for your project. Good luck!

P.S: If you need more cars, you can check my other vehicle assets on the Unity Asset Store, perhaps you could find
something useful for your game. Best regards, Mena.
*/

using System;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{

    //CAR SETUP

      [Space(20)]
      //[Header("CAR SETUP")]
      [Space(10)]
      [Range(20, 190)]
      public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
      [Range(10, 120)]
      public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
      [Range(1, 10)]
      public int accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
      [Space(10)]
      [Range(10, 45)]
      public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
      [Range(0.1f, 1f)]
      public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
      [Space(10)]
      [Range(100, 600)]
      public int brakeForce = 350; // The strength of the wheel brakes.
      [Range(1, 10)]
      public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
      [Range(1, 10)]
      public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
      [Space(10)]
      public Vector3 bodyMassCenter; // This is a vector that contains the center of mass of the car. I recommend to set this value
                                    // in the points x = 0 and z = 0 of your car. You can select the value that you want in the y-axis,
                                    // however, you must notice that the higher this value is, the more unstable the car becomes.
                                    // Usually the y value goes from 0 to 1.5.
    //WHEELS

      //[Header("WHEELS")]

      /*
      The following variables are used to store the wheels' data of the car. We need both the mesh-only game objects and wheel
      collider components of the wheels. The wheel collider components and 3D meshes of the wheels cannot come from the same
      game object; they must be separate game objects.
      */
      public GameObject frontLeftMesh;
      public WheelCollider frontLeftCollider;
      [Space(10)]
      public GameObject frontRightMesh;
      public WheelCollider frontRightCollider;
      [Space(10)]
      public GameObject rearLeftMesh;
      public WheelCollider rearLeftCollider;
      [Space(10)]
      public GameObject rearRightMesh;
      public WheelCollider rearRightCollider;
      public bool IsPlayer;
    //PARTICLE SYSTEMS

      [Space(20)]
      //[Header("EFFECTS")]
      [Space(10)]
      //The following variable lets you set up particle systems in your car
      public bool useEffects;

      // The following particle systems are used as tire smoke when the car drifts.
      public ParticleSystem RLWParticleSystem;
      public ParticleSystem RRWParticleSystem;

      [Space(10)]
      // The following trail renderers are used as tire skids when the car loses traction.
      public TrailRenderer RLWTireSkid;
      public TrailRenderer RRWTireSkid;

      // The following particle systems are used as nitro boost.
      public ParticleSystem NitroParticleSystem;

    //SPEED TEXT (UI)

      [Space(20)]
      //[Header("UI")]
      [Space(10)]
      //The following variable lets you set up a UI text to display the speed of your car.
      public bool useUI;
      public Text carSpeedText; // Used to store the UI object that is going to show the speed of the car.

    //SOUNDS

      [Space(20)]
      //[Header("Sounds")]
      [Space(10)]
      //The following variable lets you set up sounds for your car such as the car engine or tire screech sounds.
      public bool useSounds;
      public AudioSource carEngineSound; // This variable stores the sound of the car engine.
      public AudioSource tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
      float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.

    //CONTROLS

      [Space(20)]
      //[Header("CONTROLS")]
      [Space(10)]
      //The following variables lets you set up touch controls for mobile devices.
      public bool useTouchControls;
      public GameObject throttleButton;
      public PrometeoTouchInput throttlePTI;
      public GameObject reverseButton;
      public PrometeoTouchInput reversePTI;
      public GameObject turnRightButton;
      public PrometeoTouchInput turnRightPTI;
      public GameObject turnLeftButton;
      public PrometeoTouchInput turnLeftPTI;
      public GameObject handbrakeButton;
      public PrometeoTouchInput handbrakePTI;

    //NITRO BOOST

      [Space(20)]
      //[Header("Nitro Boost")]
      [Space(10)]
      public float nitroCapacity = 100f;
      public float nitroConsumptionRate = 10f;
      public float nitroRechargeRate = 5f;
      public float nitroBoostMultiplier = 2f;
      

    //CAR DATA

      [HideInInspector]
      public float carSpeed; // Used to store the speed of the car.
      [HideInInspector]
      public bool isDrifting; // Used to know whether the car is drifting or not.
      [HideInInspector]
      public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.

    //PRIVATE VARIABLES

      /*
      IMPORTANT: The following variables should not be modified manually since their values are automatically given via script.
      */
      private Rigidbody carRigidbody; // Stores the car's rigidbody.
      private float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
      private float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
      private float driftingAxis;
      private float localVelocityZ;
      private float localVelocityX;
      private bool deceleratingCar;
      private bool touchControlsSetup;
      /*
      The following variables are used to store information about sideways friction of the wheels (such as
      extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
      make the car to start drifting.
      */
      private WheelFrictionCurve FLwheelFriction;
      private float FLWextremumSlip;
      private WheelFrictionCurve FRwheelFriction;
      private float FRWextremumSlip;
      private WheelFrictionCurve RLwheelFriction;
      private float RLWextremumSlip;
      private WheelFrictionCurve RRwheelFriction;
      private float RRWextremumSlip;
      private float currentNitro;
      private bool isNitroActive;



    // Start is called before the first frame update
    private void Start()
    {
        //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
        //gameObject. Also, we define the center of mass of the car with the Vector3 given
        //in the inspector.
      currentNitro = nitroCapacity;
      carRigidbody = gameObject.GetComponent<Rigidbody>();
      carRigidbody.centerOfMass = bodyMassCenter;
      IsPlayer = false;
      //Initial setup to calculate the drift value of the car. This part could look a bit
      //complicated, but do not be afraid, the only thing we're doing here is to save the default
      //friction values of the car wheels so we can set an appropriate drifting value later.
      FLwheelFriction = new WheelFrictionCurve
      {
        extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip
      };
      FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
      FRwheelFriction = new WheelFrictionCurve
      {
        extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip
      };
      FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
      RLwheelFriction = new WheelFrictionCurve
      {
        extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip
      };
      RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
      RRwheelFriction = new WheelFrictionCurve
      {
        extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip
      };
      RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        // We save the initial pitch of the car engine sound.
        if(carEngineSound != null){
          initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        // We invoke 2 methods inside this script. CarSpeedUI() changes the text of the UI object that stores
        // the speed of the car and CarSounds() controls the engine and drifting sounds. Both methods are invoked
        // in 0 seconds, and repeatedly called every 0.1 seconds.
        if(useUI){
          InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
        }else{
          if(carSpeedText != null){
            carSpeedText.text = "0";
          }
        }

        if(useSounds){
          InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }else{
          if(carEngineSound != null){
            carEngineSound.Stop();
          }
          if(tireScreechSound != null){
            tireScreechSound.Stop();
          }
        }

        if(!useEffects){
          if(RLWParticleSystem != null){
            RLWParticleSystem.Stop();
          }
          if(RRWParticleSystem != null){
            RRWParticleSystem.Stop();
          }
          if(RLWTireSkid != null){
            RLWTireSkid.emitting = false;
          }
          if(RRWTireSkid != null){
            RRWTireSkid.emitting = false;
          }
          if(NitroParticleSystem != null)
          {
              NitroParticleSystem.Stop();
          }
        }

        if (!useTouchControls) return;
        if(throttleButton != null && reverseButton != null &&
           turnRightButton != null && turnLeftButton != null
           && handbrakeButton != null){

          throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
          reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
          turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
          turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
          handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
          touchControlsSetup = true;

        }else{
          var ex = "Touch controls are not completely set up. You must drag and drop your scene buttons in the" +
                   "PrometeoCarController component.";
          Debug.LogWarning(ex);
        }

    }

    // Update is called once per frame
    private void Update()
    {

      //CAR DATA

      // We determine the speed of the car.
      carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
      // Save the local velocity of the car in the x-axis. Used to know if the car is drifting.
      localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
      // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
      localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

      //CAR PHYSICS

      /*
      The next part is regarding the car controller. First, it checks if the user wants to use touch controls (for
      mobile devices) or analog input controls (WASD + Space).

      The following methods are called whenever a certain key is pressed. For example, in the first 'if' we call the
      method GoForward() if the user has pressed W.

      In this part of the code we specify what the car needs to do if the user presses W (throttle), S (reverse),
      A (turn left), D (turn right) or Space bar (handbrake).
      */
      if (useTouchControls && touchControlsSetup){
        if(throttlePTI.buttonPressed){
          CancelInvoke(nameof(DecelerateCar));
          deceleratingCar = false;
          GoForward();
        }
        if(reversePTI.buttonPressed){
          CancelInvoke(nameof(DecelerateCar));
          deceleratingCar = false;
          GoReverse();
        }

        if(turnLeftPTI.buttonPressed){
          TurnLeft();
        }
        if(turnRightPTI.buttonPressed){
          TurnRight();
        }
        if(handbrakePTI.buttonPressed){
          CancelInvoke(nameof(DecelerateCar));
          deceleratingCar = false;
          Handbrake();
        }
        if(!handbrakePTI.buttonPressed){
          RecoverTraction();
        }
        if((!throttlePTI.buttonPressed && !reversePTI.buttonPressed)){
          ThrottleOff();
        }
        if((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar){
          InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
          deceleratingCar = true;
        }
        if(!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f){
          ResetSteeringAngle();
        }

      }else{
        if (IsPlayer)
        {
          if (Input.GetKey(KeyCode.LeftShift) && currentNitro > 0 && Input.GetKey(KeyCode.W)){
            isNitroActive = true;
            currentNitro -= nitroConsumptionRate * Time.deltaTime;
            if (currentNitro < 0)
            {
              currentNitro = 0;
              isNitroActive = false;
            }
          }
          else
          {
            isNitroActive = false;
            if (currentNitro < nitroCapacity)
            {
              currentNitro += nitroRechargeRate * Time.deltaTime;
              if (currentNitro > nitroCapacity)
              {
                currentNitro = nitroCapacity;
              }
            }
          }


          if (Input.GetKey(KeyCode.W)){
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
            GoForward();
          }
          if(Input.GetKey(KeyCode.S)){
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
            GoReverse();
          }

          if(Input.GetKey(KeyCode.A)){
            TurnLeft();
          }
          if(Input.GetKey(KeyCode.D)){
            TurnRight();
          }
          if(Input.GetKey(KeyCode.Space)){
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
            Handbrake();
          }
          if(Input.GetKeyUp(KeyCode.Space)){
            RecoverTraction();
          }
          if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
            ThrottleOff();
          }
          if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
            InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
            deceleratingCar = true;
          }
          if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f){
            ResetSteeringAngle();
          }
        }
        else
        {
          AiControl();
        }
      }
      // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
      AnimateWheelMeshes();
    }

    private void AiControl()
    {
      CancelInvoke(nameof(DecelerateCar));
      deceleratingCar = false;
      GoForward();
    }

    // This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
    public void CarSpeedUI(){

      if(useUI){
          try{
            float absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
          }catch(Exception ex){
            Debug.LogWarning(ex);
          }
      }

    }

    // This method controls the car sounds. For example, the car engine will sound slow when the car speed is low because the
    // pitch of the sound will be at its lowest point. On the other hand, it will sound fast when the car speed is high because
    // the pitch of the sound will be the sum of the initial pitch + the car speed divided by 100f.
    // Apart from that, the tireScreechSound will play whenever the car starts drifting or losing traction.
    public void CarSounds(){
      if(useSounds){
        try{
          if(carEngineSound != null){
            var engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
            carEngineSound.pitch = engineSoundPitch;
          }
          if((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)){
            if(!tireScreechSound.isPlaying){
              tireScreechSound.Play();
            }
          }else if((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)){
            tireScreechSound.Stop();
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }
      }else{
        if(carEngineSound != null && carEngineSound.isPlaying){
          carEngineSound.Stop();
        }
        if(tireScreechSound != null && tireScreechSound.isPlaying){
          tireScreechSound.Stop();
        }
      }

    }

    //
    //STEERING METHODS
    //

    //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
    private void TurnLeft(){
      steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis < -1f){
        steeringAxis = -1f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
    private void TurnRight(){
      steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis > 1f){
        steeringAxis = 1f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
    // on the steeringSpeed variable.
    private void ResetSteeringAngle(){
      steeringAxis = steeringAxis switch
      {
        < 0f => steeringAxis + (Time.deltaTime * 10f * steeringSpeed),
        > 0f => steeringAxis - (Time.deltaTime * 10f * steeringSpeed),
        _ => steeringAxis
      };
      if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f){
        steeringAxis = 0f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
    private void AnimateWheelMeshes(){
      try{
        frontLeftCollider.GetWorldPose(out var FLWPosition, out var FLWRotation);
        frontLeftMesh.transform.position = FLWPosition;
        frontLeftMesh.transform.rotation = FLWRotation;

        frontRightCollider.GetWorldPose(out var FRWPosition, out var FRWRotation);
        frontRightMesh.transform.position = FRWPosition;
        frontRightMesh.transform.rotation = FRWRotation;

        rearLeftCollider.GetWorldPose(out var RLWPosition, out var RLWRotation);
        rearLeftMesh.transform.position = RLWPosition;
        rearLeftMesh.transform.rotation = RLWRotation;

        rearRightCollider.GetWorldPose(out var RRWPosition, out var RRWRotation);
        rearRightMesh.transform.position = RRWPosition;
        rearRightMesh.transform.rotation = RRWRotation;
      }catch(Exception ex){
        Debug.LogWarning(ex);
      }
    }

    //
    //ENGINE AND BRAKING METHODS
    //

    // This method apply positive torque to the wheels in order to go forward.
    public void GoForward(){
      //If the forces applied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car is losing traction, then the car will start emitting particle systems.
      isDrifting = Mathf.Abs(localVelocityX) > 2.5f;

      DriftCarPS();
      // The following part sets the throttle power to 1 smoothly.
      throttleAxis = throttleAxis + (Time.deltaTime * 3f);
      if(throttleAxis > 1f){
        throttleAxis = 1f;
      }
      //If the car is going backwards, then apply brakes in order to avoid strange
      //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
      //is safe to apply positive torque to go forward.
      if(localVelocityZ < -1f){
        Brakes();
      }else{
        if(Mathf.RoundToInt(carSpeed) < (isNitroActive ? 10000f : maxSpeed)){
          //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f * (isNitroActive ? 2 : 1)) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f * (isNitroActive ? 2 : 1)) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f * (isNitroActive ? 2 : 1)) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f * (isNitroActive ? 2 : 1)) * throttleAxis;
        }else {
          // If the maxSpeed has been reached, then stop applying torque to the wheels.
          // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
          // could be a bit higher than expected.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    // This method apply negative torque to the wheels in order to go backwards.
    public void GoReverse(){
      //If the forces applied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car is losing traction, then the car will start emitting particle systems.
      isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
      DriftCarPS();
      // The following part sets the throttle power to -1 smoothly.
      throttleAxis = throttleAxis - (Time.deltaTime * 3f);
      if(throttleAxis < -1f){
        throttleAxis = -1f;
      }
      //If the car is still going forward, then apply brakes in order to avoid strange
      //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
      //is safe to apply negative torque to go reverse.
      if(localVelocityZ > 1f){
        Brakes();
      }else{
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
          //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
          // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
          // could be a bit higher than expected.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
    private void ThrottleOff(){
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
    }

    // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
    // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
    // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
    public void DecelerateCar(){
      isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
      DriftCarPS();
      // The following part resets the throttle power to 0 smoothly.
      if(throttleAxis != 0f)
      {
        throttleAxis = throttleAxis switch
        {
          > 0f => throttleAxis - (Time.deltaTime * 10f),
          < 0f => throttleAxis + (Time.deltaTime * 10f),
          _ => throttleAxis
        };
        if(Mathf.Abs(throttleAxis) < 0.15f){
          throttleAxis = 0f;
        }
      }
      carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
      // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
      // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
      // also cancel to invoke of this method.
      if (!(carRigidbody.velocity.magnitude < 0.25f)) return;
      carRigidbody.velocity = Vector3.zero;
      CancelInvoke(nameof(DecelerateCar));
    }

    // This function applies brake torque to the wheels according to the brake force given by the user.
    private void Brakes(){
      frontLeftCollider.brakeTorque = brakeForce;
      frontRightCollider.brakeTorque = brakeForce;
      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;
    }

    // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
    // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
    // it is high, then you could make the car to feel like going on ice.
    private void Handbrake(){
      CancelInvoke(nameof(RecoverTraction));
      // We are going to start losing traction smoothly, there is where our 'driftingAxis' variable takes
      // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
      // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
      driftingAxis = driftingAxis + (Time.deltaTime);
      var secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

      if(secureStartingPoint < FLWextremumSlip){
        driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
      }
      if(driftingAxis > 1f){
        driftingAxis = 1f;
      }
      //If the forces applied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car lost its traction, then the car will start emitting particle systems.
      isDrifting = Mathf.Abs(localVelocityX) > 2.5f;
      //If the 'driftingAxis' value is not 1f, it means that the wheels have not reached their maximum drifting
      //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
      // = 1f.
      if(driftingAxis < 1f){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;
      }

      // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
      // and, as a consequence, the car starts to emit trails to simulate the wheel skids.
      isTractionLocked = true;
      DriftCarPS();

    }

    // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
    // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
    private void DriftCarPS(){

      if(useEffects){
        try{
          if(isDrifting){
            RLWParticleSystem.Play();
            RRWParticleSystem.Play();
          }else{
            RLWParticleSystem.Stop();
            RRWParticleSystem.Stop();
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }

        try{
          if((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f){
            RLWTireSkid.emitting = true;
            RRWTireSkid.emitting = true;
          }else {
            RLWTireSkid.emitting = false;
            RRWTireSkid.emitting = false;
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }
        try
        {

            if (isNitroActive)
            {
                NitroParticleSystem.Play();
            }
            else
            {
                NitroParticleSystem.Stop();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
      }else{
        if(RLWParticleSystem){
          RLWParticleSystem.Stop();
        }
        if(RRWParticleSystem){
          RRWParticleSystem.Stop();
        }
        if(RLWTireSkid){
          RLWTireSkid.emitting = false;
        }
        if(RRWTireSkid){
          RRWTireSkid.emitting = false;
        }
        if (NitroParticleSystem)
        {
            NitroParticleSystem.Stop();
        }
      }

    }

    // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
    public void RecoverTraction(){
      isTractionLocked = false;
      driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
      if(driftingAxis < 0f){
        driftingAxis = 0f;
      }

      //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
      //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
      // car's grip.
      if(FLwheelFriction.extremumSlip > FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        Invoke(nameof(RecoverTraction), Time.deltaTime);

      }else if (FLwheelFriction.extremumSlip < FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        driftingAxis = 0f;
      }
    }

    public void EnableAiControl()
    {
      IsPlayer = true;
    }
    public void DisableAiControl()
    {
      IsPlayer = false;
    }
}
