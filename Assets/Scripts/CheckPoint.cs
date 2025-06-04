using System;
using Entity;
using Fusion;
using Scripts;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int check;
    public bool finish;

    public void Awake()
    {
        check = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 隐藏自身所有渲染器
        var playerController = other.GetComponentInParent<PlayerController>();
        if (!playerController) return;
        playerController.Check(transform.position,playerController.car.transform.rotation,check);
        check = 0;
        if (finish)
        {
            playerController.Finish();
        }
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }
    public void Restart()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
    }
}