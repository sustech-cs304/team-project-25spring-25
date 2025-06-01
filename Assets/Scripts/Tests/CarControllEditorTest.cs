using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CarControllEditorTest
{
    private GameObject carObject;
    private TestCar testCar;
    private Rigidbody carRigidbody;

    [SetUp]
    public void Setup()
    {
        // 创建测试用的游戏对象
        carObject = new GameObject("TestCar");
        testCar = carObject.AddComponent<TestCar>();
        carRigidbody = carObject.AddComponent<Rigidbody>();
        
        // 初始化必要的组件
        testCar.carRigidbody = carRigidbody;
        testCar.bodyMassCenter = Vector3.zero;
        
        // 创建并设置车轮碰撞器
        testCar.frontLeftCollider = new GameObject("FrontLeftWheel").AddComponent<WheelCollider>();
        testCar.frontRightCollider = new GameObject("FrontRightWheel").AddComponent<WheelCollider>();
        testCar.rearLeftCollider = new GameObject("RearLeftWheel").AddComponent<WheelCollider>();
        testCar.rearRightCollider = new GameObject("RearRightWheel").AddComponent<WheelCollider>();

        // 创建漂移特效组件
        var leftTireObject = new GameObject("LeftTireSkid");
        var rightTireObject = new GameObject("RightTireSkid");
        testCar.RLWTireSkid = leftTireObject.AddComponent<TrailRenderer>();
        testCar.RRWTireSkid = rightTireObject.AddComponent<TrailRenderer>();
        leftTireObject.transform.parent = carObject.transform;
        rightTireObject.transform.parent = carObject.transform;

        // 创建氮气特效
        var nitroObject = new GameObject("NitroEffect");
        testCar.NitroParticleSystem = nitroObject.AddComponent<ParticleSystem>();
        nitroObject.transform.parent = carObject.transform;

        // 创建音效组件
        testCar.carEngineSound = carObject.AddComponent<AudioSource>();
        testCar.tireScreechSound = carObject.AddComponent<AudioSource>();
    }

    [TearDown]
    public void TearDown()
    {
        // 清理测试对象
        Object.DestroyImmediate(carObject);
    }

    [Test]
    public void TestCarInitialization()
    {
        // 测试车辆初始化
        Assert.IsNotNull(testCar);
        Assert.IsNotNull(testCar.carRigidbody);
        Assert.IsFalse(testCar.isNitroActive);
        Assert.IsFalse(testCar.isDrifting);
    }

    [Test]
    public void TestSteeringControl()
    {
        // 测试转向控制
        testCar.SetSteeringAngle(1f);
        Assert.AreEqual(1f, testCar.steeringAxis);
        
        testCar.SetSteeringAngle(-1f);
        Assert.AreEqual(-1f, testCar.steeringAxis);
        
        testCar.SetSteeringAngle(2f); // 测试超出范围的值
        Assert.AreEqual(1f, testCar.steeringAxis);
    }

    [Test]
    public void TestNitroSystem()
    {
        // 激活氮气
        testCar.TryNitroActive(true);
        Assert.IsTrue(testCar.isNitroActive);
        
        // 关闭氮气
        testCar.TryNitroActive(false);
        Assert.IsFalse(testCar.isNitroActive);
    }

    [Test]
    public void TestDriftSystem()
    {
        // 测试漂移系统
        Assert.IsFalse(testCar.isDrifting);
        
        // 激活漂移
        testCar.TryDrift(true);
        Assert.IsTrue(testCar.isDrifting);
        
        // 关闭漂移
        testCar.TryDrift(false);
        Assert.IsFalse(testCar.isDrifting);
    }


    [UnityTest]
    public IEnumerator TestCarMovement()
    {
        // 测试前进
        for (int i = 0; i < 30; i++)
        {
            testCar.GoForward();
            yield return null;
        }
        
        Assert.Greater(testCar.rearLeftCollider.rotationSpeed, -1f);
        
        // 先停止前进
        testCar.ThrottleOff();
        
        // 等待车辆减速
        for (int i = 0; i < 30; i++)
        {
            yield return null;
        }
        
        // 确保车辆几乎停止
        Assert.Less(testCar.rearLeftCollider.rotationSpeed, 0.1f);
        
        // 测试后退
        for (int i = 0; i < 30; i++)
        {
            testCar.GoReverse();
            yield return null;
        }
        
        // 验证后退速度
        Assert.Less(testCar.rearLeftCollider.rotationSpeed, 1f);
    }
}
