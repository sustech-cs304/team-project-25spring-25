using UnityEngine;
using UnityEngine.UI;

public class MiniMapSystem : MonoBehaviour
{
    public static MiniMapSystem Instance { get; private set; }
    
    [Header("Settings")]
    public LayerMask miniMapLayer;         // 小地图能看到的图层
    public float miniMapHeight = 50f;      // 小地图摄像头高度
    public float miniMapSize = 100f;       // 小地图显示范围
    public bool rotateWithPlayer = true;   // 是否随玩家旋转
    
    [Header("References")]
    public Camera miniMapCamera;           // 小地图摄像头
    public RawImage miniMapImage;          // UI上的小地图图像
    public GameObject playerMarkerPrefab;  // 玩家标记预制体
    public GameObject enemyMarkerPrefab;   // 敌人标记预制体
    
    private Transform playerCar;           // 玩家车辆
    private GameObject playerMarker;       // 玩家标记实例
    private readonly System.Collections.Generic.Dictionary<Car, GameObject> enemyMarkers = 
        new System.Collections.Generic.Dictionary<Car, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void LateUpdate()
    {
        if (playerCar == null) return;
        
        // 更新小地图摄像头位置
        Vector3 newPosition = playerCar.position;
        newPosition.y = miniMapHeight;
        miniMapCamera.transform.position = newPosition;
        // 更新玩家标记位置（新增关键代码）
        if (playerMarker != null)
        {
            playerMarker.transform.position = new Vector3(
                playerCar.position.x,
                miniMapHeight-5f, // 保持与SetPlayerCar中一致的高度偏移
                playerCar.position.z
            );
            playerMarker.transform.rotation = Quaternion.Euler(90, playerCar.transform.eulerAngles.y, 0);
        }
        // 如果需要旋转小地图
        if (rotateWithPlayer)
        {
            miniMapCamera.transform.rotation = Quaternion.Euler(90, playerCar.eulerAngles.y, 0);
            // 旋转UI图像以保持玩家方向向上
            // miniMapImage.transform.rotation = Quaternion.Euler(0, 0, -playerCar.eulerAngles.y);
        }
        else
        {
            miniMapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        
        // 更新所有敌人标记位置
        foreach (var kvp in enemyMarkers)
        {
            if (kvp.Key && kvp.Value)
            {
                kvp.Value.transform.position = new Vector3(
                    kvp.Key.transform.position.x,
                    miniMapHeight-5f,
                    kvp.Key.transform.position.z
                );
                kvp.Value.transform.rotation = Quaternion.Euler(90, kvp.Key.transform.eulerAngles.y, 0);
            }
        }
    }

    // 设置玩家车辆
    public void SetPlayerCar(Transform carTransform)
    {
        playerCar = carTransform;
        
        // 创建玩家标记
        if (playerMarker != null) Destroy(playerMarker);
        // 实例化3D标记（必须挂到普通父物体下，不能是UI）
        playerMarker = Instantiate(playerMarkerPrefab, transform); // 关键修改点
    
        // 设置标记位置（与小地图摄像机同高度）
        playerMarker.transform.position = new Vector3(
            carTransform.position.x,
            miniMapHeight-5f, // 使用统一的摄像机高度
            carTransform.position.z
        );
        playerMarker.transform.rotation = Quaternion.Euler(90, playerCar.transform.eulerAngles.y, 0); // 固定朝北
        
        // 设置小地图摄像头
        miniMapCamera.orthographicSize = miniMapSize;
        miniMapCamera.cullingMask = miniMapLayer;
    }
    
    // 添加敌人车辆标记
    public void AddEnemyCar(Car enemyCar)
    {
        if (enemyMarkers.ContainsKey(enemyCar)) return;
        
        GameObject marker = Instantiate(enemyMarkerPrefab, transform);
    
        // 设置初始位置和旋转（同步车辆朝向）
        marker.transform.position = new Vector3(
            enemyCar.transform.position.x,
            miniMapHeight - 5f,
            enemyCar.transform.position.z
        );
        marker.transform.rotation = Quaternion.Euler(90, enemyCar.transform.eulerAngles.y, 0);
        enemyMarkers.Add(enemyCar, marker);
    }

    // 移除车辆标记
    public void RemoveCar(Car car)
    {
        if (car == playerCar.GetComponent<Car>())
        {
            if (playerMarker != null) Destroy(playerMarker);
            playerMarker = null;
            playerCar = null;
        }
        else if (enemyMarkers.ContainsKey(car))
        {
            Destroy(enemyMarkers[car]);
            enemyMarkers.Remove(car);
        }
    }

    // 清除所有标记
    public void ClearAllMarkers()
    {
        if (playerMarker != null) Destroy(playerMarker);
        playerMarker = null;
        playerCar = null;
        
        foreach (var marker in enemyMarkers.Values)
        {
            if (marker != null) Destroy(marker);
        }
        enemyMarkers.Clear();
    }
}