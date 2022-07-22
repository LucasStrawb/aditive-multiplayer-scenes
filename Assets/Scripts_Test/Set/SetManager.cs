using EasyButtons;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class SetManager : SimulationBehaviour
    {
        public static SetManager Instance => _instance;
        private static SetManager _instance;

        [SerializeField] SetData[] _setsData;

        private static readonly List<Scene> _scenes = new();
        public static Scene CurrentScene => _scenes[_currentScene];
        private static int _currentScene = -1;
        public static PhysicsScene CurrentPhysicsScene => CurrentScene.GetPhysicsScene();

        public static System.Action<Scene> OnSceneChanged;

        private void Awake()
        {
            _instance = this;
        }

        [Button]
        public void SpawnSet()
        {
            var parms = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            var sceneCount = _scenes.Count;
            var scene = SceneManager.CreateScene("Set " + sceneCount, parms);
            _scenes.Add(scene);

            SetCurrentScene(sceneCount);

            SpawnSetMoc(sceneCount);
        }

        private void SpawnSetMoc(int value)
        {
            if (value >= _setsData.Length)
                return;

            foreach (var prefab in _setsData[value].GameObjects)
            {
                var go = Instantiate(prefab);
                go.AddComponent<RendererSetController>();
            }
        }

        [Button]
        public void SetCurrentSceneAsOne()
        {
            SetCurrentScene(1);
        }

        [Button]
        public void SetCurrentSceneAsZero()
        {
            SetCurrentScene(0);
        }

        [Button]
        public void SetCurrentScene(int value)
        {
            _currentScene = value;
            SceneManager.SetActiveScene(CurrentScene);
            OnSceneChanged?.Invoke(CurrentScene);
        }

        public void ToggleScene(int sceneIndex)
        {
            if (!Runner.SceneManager().LoadedScenes.Contains(sceneIndex))
                Runner.SceneManager().AddScene(sceneIndex);
            else
                Runner.SceneManager().RemoveScene(sceneIndex);
        }

        public void ReloadScenesButton()
        {
            if (Runner.SceneManager() && Runner.IsServer)
                Runner.SceneManager().StartReloadScenes();
        }
    }
}