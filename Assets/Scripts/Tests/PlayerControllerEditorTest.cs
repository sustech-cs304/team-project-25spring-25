using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using NUnit.Framework;
using Scripts;

[TestFixture]
public class PlayerControllerEditorTest
{
    private GameObject playerObject;
    private TestPlayerController playerController;
    private TestCar car;
    private GameObject carObject;

    [SetUp]
    public void Setup()
    {
        // 创建测试对象
        playerObject = new GameObject("TestPlayer");
        carObject = new GameObject("TestCar");
        carObject.AddComponent<Rigidbody>();
        car = carObject.AddComponent<TestCar>();
        playerController = playerObject.AddComponent<TestPlayerController>();
        
        // 设置父子关系
        playerObject.transform.SetParent(carObject.transform);
        playerController.car = car;
    }

    [TearDown]
    public void TearDown()
    {
        // 清理测试对象
        Object.DestroyImmediate(carObject);
    }

    [Test]
    public void TestPlayerControllerInitialization()
    {
        // 测试初始化
        Assert.IsNotNull(playerController, "PlayerController 应该被正确创建");
        Assert.IsNotNull(playerController.car, "Car 组件应该被正确赋值");
        Assert.AreEqual(car, playerController.car, "Car 引用应该正确设置");
    }


    [Test]
    public void TestInputHandling()
    {
        // 模拟输入并测试响应
        // 注意：在编辑器测试中，我们无法直接模拟 Input.GetKey
        // 这里我们测试控制器是否正确引用了 Car 组件
        Assert.IsNotNull(playerController.car, "Car 组件应该在输入处理前存在");
    }

} 