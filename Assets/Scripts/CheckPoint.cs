using Fusion;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 隐藏自身所有渲染器
        if (other.gameObject.GetComponentInParent<Car>().isPlayer)
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

    }
}