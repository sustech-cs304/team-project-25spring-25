using Manager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Car car;
  public Transform[] checkPointTransforms;
  public int scoreCheck;
  public int maxScoreCheck;
  public bool over;
  private void Awake()
  {
    over = false;
    car = GetComponent<Car>();
    scoreCheck = 0;
    maxScoreCheck = 0;
  }
  public void Check(Vector3 pos, Quaternion rot,int check)
  {
    scoreCheck += check;
    car.currentNitro += check * 20f;
    car.SetResetValue(pos, rot);
  }
  public void SetCheckPoints(Transform[] checkPointTransforms)
  {
    this.checkPointTransforms = checkPointTransforms;
    for (var i = 0; i < checkPointTransforms.Length; i++)
    {
      var checkPointTransform = checkPointTransforms[i];
      if (checkPointTransform.gameObject.activeSelf)
      {
        maxScoreCheck++;
      }
    }
  }

  private void Update()
  {
    if (checkPointTransforms == null || over) return;
    var isAccelerating = Input.GetKey(KeyCode.W);
    var isBraking = Input.GetKey(KeyCode.S);
    var isTurningLeft = Input.GetKey(KeyCode.A);
    var isTurningRight = Input.GetKey(KeyCode.D);
    var isHandbraking = Input.GetKey(KeyCode.Space);
    var isNitroPressed = Input.GetKey(KeyCode.LeftShift);
    var isReset = Input.GetKey(KeyCode.R);
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
    if (isReset) car.ResetCar();
    car.TryDrift(isHandbraking);
    car.TryNitroActive(isNitroPressed);
    car.UpdateData();
  }

  public void SetCar()
  {
    car = GetComponent<Car>();
  }

  public void Finish()
  {
    over = true;
    GameManager.Instance.GameOver(scoreCheck, maxScoreCheck);
  }
}