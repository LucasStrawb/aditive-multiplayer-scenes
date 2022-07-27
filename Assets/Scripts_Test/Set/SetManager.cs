using EasyButtons;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class SetManager : MonoBehaviour
    {
        public static SetManager Instance => _instance;
        private static SetManager _instance;

        public static string CurrentSceneName => ($"Set {_currentScene}");
        private static int _currentScene = -1;

        public static System.Action<string> OnSceneChanged;

        private void Awake()
        {
            _instance = this;
            //CustomSceneLoader.OnSceneLoaded += (value) => SetCurrentScene(value);
        }

        [Button]
        public void SetCurrentScene(int value)
        {
            _currentScene = value;
            //var scene = SceneManager.GetSceneByName(CurrentSceneName);
            //SceneManager.SetActiveScene(scene);
            //OnSceneChanged?.Invoke(CurrentSceneName);
        }

        public void ToggleSet(int value)
        {
            Launcher.Runner.SceneManager().ToggleScene(value);
        }
    }
}