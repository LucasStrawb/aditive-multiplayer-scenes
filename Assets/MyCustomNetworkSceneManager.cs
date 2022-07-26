using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class MyCustomNetworkSceneManager : NetworkBehaviour
    {
        public string SceneName = "Empty";

        [Networked, Capacity(64)] public NetworkLinkedList<int> CurrentScenes => default; // Synced

        public List<int> InterestedInScenes = new(); // Client Side
        public Dictionary<int, Scene> LoadedScenes = new(); // Client Side

        public Action<int, Scene> OnSceneLoaded;

        public override void Spawned()
        {
            if (Runner.SceneManager())
            {
                Runner.Despawn(Object);
                return;
            }
            DontDestroyOnLoad(this);
            Runner.SetSceneManager(this);
        }

        public void ToggleSet(int index)
        {
            SetCurrentScene(index);
        }
        
        public void SetCurrentScene(int index)
        {
            if (!CurrentScenes.Contains(index))
            {
                CurrentScenes.Add(index);
            }

            if (!InterestedInScenes.Contains(index))
            {
                InterestedInScenes.Add(index);

                OnSceneLoaded += (sceneIndex, scene) =>
                {
                    Debug.Log("OnSceneLoaded");
                    if (sceneIndex == index)
                        LoadedScenes.Add(sceneIndex, scene);
                };
            }
        }
        public void UnloadOutdatedScenes()
        {
        }

        public bool IsSceneUpdated(out int sceneRef)
        {
            foreach (var scene in CurrentScenes)
            {               
                if (LoadedScenes.ContainsKey(scene))
                    continue;

                if (InterestedInScenes.Contains(scene))
                {
                    sceneRef = scene;
                    return false;
                }
            }
            sceneRef = SceneRef.None;
            return true;
        }


        #region SinglePlayer
        public void LoadSetScene(int index)
        {
            _ = LoadSetSceneAsync(index);
        }

        private async Task LoadSetSceneAsync(int index)
        {
            var op = await SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            op.completed += (operation) =>
            {
                var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                LoadedScenes.Add(index, scene);
            };
        }

        public void UnloadSetScene(int index)
        {
            _ = UnloadSetSceneAsync(index);
        }

        public async Task UnloadSetSceneAsync(int index)
        {
            var scene = LoadedScenes[index];
            var op = await SceneManager.UnloadSceneAsync(scene);
            op.completed += (operation) =>
            {
                LoadedScenes.Remove(index);
            };
        }
        #endregion
    }
}