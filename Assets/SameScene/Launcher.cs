using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SameScene
{
    public class Launcher : MonoBehaviour
    {
        public static Action<GameMode> OnCurrentGameModeChanged;
        public static Action OnInitialized;

        [SerializeField] private string _sessionName = "Test";

        public static NetworkRunner Runner => _runner;
        private static NetworkRunner _runner;

        [SerializeField] private List<SimulationBehaviour> _simulations = new();

        private void Awake()
        {
            if (_runner != default)
            {
                Destroy(gameObject);
                return;
            }

            _runner = gameObject.AddComponent<NetworkRunner>();

            _runner.ProvideInput = true;
            Debug.Log("Starting Game");
            _runner.StartGame(new StartGameArgs()
            {
                SessionName = _sessionName,
                GameMode = GameMode.AutoHostOrClient,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Initialized = Initialized,
            });

            void Initialized(NetworkRunner runner)
            {
                Debug.Log("Game Initialized");
                foreach (var simulation in _simulations)
                    runner.AddSimulationBehaviour(simulation);
                OnInitialized?.Invoke();
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