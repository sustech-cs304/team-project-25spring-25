using Fusion;
using Manager;
using UnityEngine;

public class RpcController : NetworkBehaviour
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStartRace()
    {
        Debug.Log("游戏开始（来自 RpcController）");
        GameManager.Instance.InitNetworkRace();
    }
}