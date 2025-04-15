
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static bool IsExisted {get; private set;} = false;
    private static T _instance;

    // 确保单例对象不会被销毁
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(); // 查找现有的实例
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            DontDestroyOnLoad(_instance);
            IsExisted = true;
            return _instance;
        }
    }
    private void OnDestroy()
    {
        IsExisted = false;
    }
}