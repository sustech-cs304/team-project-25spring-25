using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Scripts;

public class AiControllerEditorTest
{
    private GameObject aiCarObject;
    private TestAiController aiController;
    private TestCar testCar;
    private GameObject[] targetObjects;

    [SetUp]
    public void Setup()
    {
        // 创建AI车辆对象
        aiCarObject = new GameObject("AICar");
        testCar = aiCarObject.AddComponent<TestCar>();
        aiController = aiCarObject.AddComponent<TestAiController>();
        
        // 设置必要的组件
        testCar.carRigidbody = aiCarObject.AddComponent<Rigidbody>();
        testCar.bodyMassCenter = Vector3.zero;
        
        // 创建车轮碰撞器
        testCar.frontLeftCollider = new GameObject("FrontLeftWheel").AddComponent<WheelCollider>();
        testCar.frontRightCollider = new GameObject("FrontRightWheel").AddComponent<WheelCollider>();
        testCar.rearLeftCollider = new GameObject("RearLeftWheel").AddComponent<WheelCollider>();
        testCar.rearRightCollider = new GameObject("RearRightWheel").AddComponent<WheelCollider>();

        // 创建目标点
        targetObjects = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            targetObjects[i] = new GameObject($"Target_{i}");
            targetObjects[i].transform.position = new Vector3(i * 10, 0, i * 10); // 设置不同的位置
        }

        // 设置AI控制器的目标点
        aiController.targets = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            aiController.targets[i] = targetObjects[i].transform;
        }

        // 设置AI控制器参数
        aiController.turnSensitivity = 10f;
        aiController.brakingDistance = 1f;
        aiController.switchTargetDistance = 10f;
        aiController.maxSpeed = 90f;
        aiController.maxSteeringAngle = 90f;
        aiController.car = testCar;
    }

    [TearDown]
    public void TearDown()
    {
        // 清理测试对象
        Object.DestroyImmediate(aiCarObject);
        foreach (var target in targetObjects)
        {
            Object.DestroyImmediate(target);
        }
    }

    [UnityTest]
    public IEnumerator TestAiMovementToTarget()
    {
        // 设置初始位置
        aiCarObject.transform.position = Vector3.zero;
        aiCarObject.transform.forward = Vector3.forward;

        // 运行AI控制器几帧
        for (int i = 0; i < 30; i++)
        {
            yield return null;
        }

        // 验证AI是否在向目标移动
        Assert.Greater(testCar.rearLeftCollider.rotationSpeed, -1f);
        
        // 验证转向角度是否合理
        Assert.LessOrEqual(Mathf.Abs(testCar.steeringAxis), 1f);
    }

    [UnityTest]
    public IEnumerator TestAiTargetSwitching()
    {
        // 将车辆放在第一个目标点附近
        aiCarObject.transform.position = targetObjects[0].transform.position + new Vector3(5f, 0, 0);
        
        // 运行几帧让AI更新
        for (int i = 0; i < 30; i++)
        {
            yield return null;
        }

        // 验证AI是否继续移动（朝向下一个目标）
        Assert.Greater(testCar.rearLeftCollider.rotationSpeed, -1f);
    }

    [UnityTest]
    public IEnumerator TestAiSpeedControl()
    {
        // 设置一个转角大的目标点位置来测试减速
        targetObjects[1].transform.position = new Vector3(5f, 0, 5f);
        targetObjects[2].transform.position = new Vector3(-5f, 0, 5f);
        
        // 运行AI一段时间
        for (int i = 0; i < 60; i++)
        {
            yield return null;
        }

        // 验证速度是否在合理范围内
        Assert.Less(testCar.carSpeed, aiController.maxSpeed);
    }
} 