using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        public NetworkRunner runnerPrefab;
        private NetworkRunner runner;

        public async void StartHost()
        {
            if (runner == null)
            {
                runner = Instantiate(runnerPrefab);
                runner.ProvideInput = true;
            }

            var sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = "RaceRoom",
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = sceneManager
            });
        }

        public async void StartClient(string address)
        {
            if (runner == null)
            {
                runner = Instantiate(runnerPrefab);
                runner.ProvideInput = true;
            }

            var sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = "RaceRoom",
                Address = NetAddress.CreateFromIpPort(address, 27015),
                SceneManager = sceneManager
            });
        }
    }
}