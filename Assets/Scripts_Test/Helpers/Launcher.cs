using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Test
{
    public class Launcher : MonoBehaviour
    {
        public static Action<GameMode> OnCurrentGameModeChanged;

        [SerializeField] private NetworkPrefabRef _networkSceneManagerPrefab;

        public static NetworkRunner Runner => _runner;
        private static NetworkRunner _runner;

        private void Awake()
        {
            if (_runner != default)
            {
                Destroy(gameObject);
                return;
            }

            _runner = gameObject.AddComponent<NetworkRunner>();

            _runner.ProvideInput = true;
            _runner.StartGame(new StartGameArgs()
            {
                SessionName = "Test",
                GameMode = GameMode.AutoHostOrClient,
                SceneManager = gameObject.AddComponent<MyCustomSceneLoader>(),
                Initialized = SpawnNetworkSceneManager
            });

            void SpawnNetworkSceneManager(NetworkRunner runner)
            {
                runner.Spawn(_networkSceneManagerPrefab);
                OnCurrentGameModeChanged?.Invoke(runner.GameMode);
            }
        }

        public static void ShutDown()
        {
            if (_runner != null)
                _runner.Shutdown();
        }
    }
}