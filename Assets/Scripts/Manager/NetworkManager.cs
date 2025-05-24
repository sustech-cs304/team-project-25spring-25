using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class NetworkManager : Singleton<NetworkManager>,INetworkRunnerCallbacks
    {
        public NetworkRunner runnerPrefab;
        public NetworkObject rpcControllerPrefab;
        public RpcController rpcController;
        private NetworkRunner runner;
        public List<PlayerRef> connectedPlayers = new List<PlayerRef>();
        
        void Awake() {
            runner = Instantiate(runnerPrefab);
            runner.ProvideInput = true;
            runner.AddCallbacks(this);
            runner.JoinSessionLobby(SessionLobby.Shared);
        }
        
        public async void StartHost()
        {
            try
            {
                await runner.StartGame(new StartGameArgs
                {
                    GameMode = GameMode.Host,
                    SessionName = "RaceRoom",
                    Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                    SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                    PlayerCount = 4,
                    IsVisible = true,    
                    IsOpen = true
                });
                var obj = runner.Spawn(rpcControllerPrefab, Vector3.zero, Quaternion.identity);
                rpcController = obj.GetComponent<RpcController>();
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }

        public NetworkRunner Runner
        {
            get => runner;
            set => runner = value;
        }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log($"Object {obj.name} exited AOI of player {player}.");
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            Debug.Log($"Object {obj.name} entered AOI of player {player}.");
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            connectedPlayers.Add(player);
            Debug.Log($"Player {player} joined. Total players: {connectedPlayers.Count}");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            connectedPlayers.Remove(player);
            Debug.Log($"Player {player} left. Remaining players: {connectedPlayers.Count}");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.LogWarning($"Runner shutdown: {shutdownReason}");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.LogError($"Disconnected from server: {reason}");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("Incoming connection request accepted.");
            request.Accept();
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.LogError($"Connection failed to {remoteAddress}: {reason}");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("Received user simulation message (not used currently).");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            Debug.Log($"Reliable data received from {player}, key={key}, length={data.Count}");
            // Handle custom data if needed
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            Debug.Log($"Reliable data progress from {player}, key={key}, progress={progress}");
        }


        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData
            {
                IsAccelerating = Input.GetKey(KeyCode.W),
                IsBraking = Input.GetKey(KeyCode.S),
                IsTurningLeft = Input.GetKey(KeyCode.A),
                IsTurningRight = Input.GetKey(KeyCode.D),
                IsHandbraking = Input.GetKey(KeyCode.Space),
                IsNitroPressed = Input.GetKey(KeyCode.LeftShift)
            };
            input.Set(data);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.LogWarning($"Input missing for player {player}. May cause prediction stutter.");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("Successfully connected to server.");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("Session list updated:");
            foreach (var session in sessionList)
            {
                Debug.Log($"Session: {session.Name}, Players: {session.PlayerCount}/{session.MaxPlayers}");
            }
            MultimodeManager.Instance.UpdateRoomListUI(sessionList,this.runner);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("Received custom authentication response.");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("Host migration triggered (not handled).");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("Scene load complete.");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("Scene loading started.");
        }
    }
}