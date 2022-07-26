using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class CustomNetworkSceneManager : NetworkBehaviour
    {
        [Networked, Capacity(64)] public NetworkLinkedList<int> CurrentScenes => default;
        public List<int> InterestedInScenes = new();
        public List<int> LoadedScenes = new();
        public Action OnUpToDate;

        public override void Spawned()
        {
            if (Runner.SceneManager())
            {
                Runner.Despawn(Object);
                return;
            }
            DontDestroyOnLoad(this);
            //Runner.SetSceneManager(this);
        }

        public void AddScene(int sceneIndex)
        {
            if ((Object.HasStateAuthority) && (CurrentScenes.Count >= CurrentScenes.Capacity || CurrentScenes.Contains(sceneIndex) || sceneIndex < 0))
            {
                Debug.LogError("Scene not added");
                return;
            }

            if (Object.HasStateAuthority) // Add scene to list if is host
            {
                CurrentScenes.Add(sceneIndex);
            }

            if (!InterestedInScenes.Contains(sceneIndex)) // Add scene to list o load on client
            {
                InterestedInScenes.Add(sceneIndex);
            }
        }

        public void RemoveScene(int sceneIndex)
        {
            if (Object.HasStateAuthority)
                CurrentScenes.Remove(sceneIndex);

            InterestedInScenes.Remove(sceneIndex);
        }

        public void ToggleScene(int sceneIndex)
        {
            //if (!Runner.SceneManager().LoadedScenes.Contains(sceneIndex))
            //    Runner.SceneManager().AddScene(sceneIndex);
            //else
            //    Runner.SceneManager().RemoveScene(sceneIndex);
        }

        public void UnloadOutdatedScenes()
        {
            var _sceneToUnload = LoadedScenes.Except(InterestedInScenes.Intersect(CurrentScenes)).FirstOrDefault();

            //if (_sceneToUnload)
            //{
            //    //SceneManager.UnloadSceneAsync($"Set {index}");
            //    SceneManager.UnloadSceneAsync(_sceneToUnload);
            //    LoadedScenes.Remove(_sceneToUnload);
            //}
        }

        public bool IsSceneUpdated(out SceneRef sceneRef)
        {
            for (int i = 0; i < CurrentScenes.Count; i++)
            {
                if (CurrentScenes[i] == default)
                    continue;
                if (LoadedScenes.Contains(CurrentScenes[i]))
                    continue;
                if (InterestedInScenes.Contains(CurrentScenes[i]))
                {
                    sceneRef = CurrentScenes[i];
                    LoadedScenes.Add(CurrentScenes[i]);
                    return false;
                }
            }
            sceneRef = SceneRef.None;
            return true;
        }
    }
}