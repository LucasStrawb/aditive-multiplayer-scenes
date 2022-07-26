using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Test
{
    public class MyCustomSceneLoader : Fusion.Behaviour, INetworkSceneManager
    {
        private static readonly WeakReference<MyCustomSceneLoader> s_currentlyLoading = new(null);

        private Task _switchSceneTask;
        private bool _currentSceneOutdated;
        protected SceneRef _currentScene;

        public NetworkRunner Runner { get; private set; }

        private int _desiredScene;

        protected virtual void LateUpdate()
        {
            if (!Runner)
                return;

            if (_switchSceneTask != null && _switchSceneTask.Status != TaskStatus.Running)
                //busy
                return;

            _currentSceneOutdated = !IsScenesUpdated();

            if (!_currentSceneOutdated)
            {
                //up to date

                //Runner.SceneManager()?.OnUpToDate?.Invoke();
                return;
            }

            if (s_currentlyLoading.TryGetTarget(out var target))
            {
                Assert.Check(target != this);
                if (!target)
                {
                    // orphaned loader?
                    s_currentlyLoading.SetTarget(null);
                }
                else
                {
                    return;
                }
            }

            _currentScene = _desiredScene;
            _currentSceneOutdated = false;

            _switchSceneTask = SwitchSceneWrapper(_currentScene);
        }

        protected static bool IsScenePathOrNameEqual(Scene scene, string nameOrPath)
        {
            return scene.path == nameOrPath || scene.name == nameOrPath;
        }

        public static bool TryGetScenePathFromBuildSettings(SceneRef sceneRef, out string path)
        {
            if (sceneRef.IsValid)
            {
                path = SceneUtility.GetScenePathByBuildIndex(sceneRef);
                if (!string.IsNullOrEmpty(path))
                {
                    return true;
                }
            }
            path = string.Empty;
            return false;
        }

        public bool IsScenePathOrNameEqual(Scene scene, SceneRef sceneRef)
        {
            if (TryGetScenePathFromBuildSettings(sceneRef, out var path))
            {
                return IsScenePathOrNameEqual(scene, path);
            }
            else
            {
                return false;
            }
        }

        public List<NetworkObject> FindNetworkObjects(Scene scene, bool disable = true, bool addVisibilityNodes = false)
        {

            var networkObjects = new List<NetworkObject>();
            var gameObjects = scene.GetRootGameObjects();
            var result = new List<NetworkObject>();

            // get all root gameobjects and move them to this runners scene
            foreach (var go in gameObjects)
            {
                networkObjects.Clear();
                go.GetComponentsInChildren(true, networkObjects);

                foreach (var sceneObject in networkObjects)
                {
                    if (sceneObject.Flags.IsSceneObject())
                    {
                        if (sceneObject.gameObject.activeInHierarchy || sceneObject.Flags.IsActivatedByUser())
                        {
                            Assert.Check(sceneObject.NetworkGuid.IsValid);
                            result.Add(sceneObject);
                        }
                    }
                }

                if (addVisibilityNodes)
                {
                    // register all render related components on this gameobject with the runner, for use with IsVisible
                    RunnerVisibilityNode.AddVisibilityNodes(go, Runner);
                }
            }

            if (disable)
            {
                // disable objects; each will be activated if there's a matching state object
                foreach (var sceneObject in result)
                {
                    sceneObject.gameObject.SetActive(false);
                }
            }

            return result;
        }

        protected bool IsScenesUpdated()
        {
            if (Runner.SceneManager() && Runner.SceneManager().Object)
            {
                Runner.SceneManager().UnloadOutdatedScenes();

                return Runner.SceneManager().IsSceneUpdated(out _desiredScene);
            }

            return true;
        }

        #region INetworkSceneObjectProvider

        void INetworkSceneManager.Initialize(NetworkRunner runner)
        {
            Initialize(runner);
        }

        void INetworkSceneManager.Shutdown(NetworkRunner runner)
        {
            Shutdown(runner);
        }

        bool INetworkSceneManager.IsReady(NetworkRunner runner)
        {
            Assert.Check(Runner == runner);
            if (_switchSceneTask != null && _switchSceneTask.Status == TaskStatus.Running || !Runner)
            {
                return false;
            }
            if (_currentSceneOutdated)
            {
                return false;
            }

            return true;
        }

        #endregion

        protected virtual void Initialize(NetworkRunner runner)
        {
            Assert.Check(!Runner);
            Runner = runner;
        }

        protected virtual void Shutdown(NetworkRunner runner)
        {
            Assert.Check(Runner == runner);

            try
            {
                // ongoing loading, dispose
                if (_switchSceneTask != null && _switchSceneTask.Status == TaskStatus.Running)
                {
                    (_switchSceneTask as IDisposable)?.Dispose();
                }
            }
            finally
            {
                Runner = null;
                _switchSceneTask = null;
                _currentScene = SceneRef.None;
                _currentSceneOutdated = false;
            }
        }

        protected async Task<IEnumerable<NetworkObject>> SwitchScene(int index)
        {
            var sceneObjects = new List<NetworkObject>();
            if (index >= 0)
            {
                var loadedScene = await LoadSceneAsset(index);
                sceneObjects = FindNetworkObjects(loadedScene, disable: false);
            }
            return sceneObjects;
        }

        private async Task<Scene> LoadSceneAsset(int index)
        {
            var scene = new Scene();
            var param = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);

            var op = await SceneManager.LoadSceneAsync(Runner.SceneManager().SceneName, param);
            op.completed += (operation) =>
            {
                scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                Runner.SceneManager().OnSceneLoaded?.Invoke(index, scene);
            };
            return scene;
        }

        private async Task SwitchSceneWrapper(int newScene)
        {
            IEnumerable<NetworkObject> sceneObjects;
            Exception error = null;

            try
            {
                Assert.Check(!s_currentlyLoading.TryGetTarget(out _));
                s_currentlyLoading.SetTarget(this);
                Runner.InvokeSceneLoadStart();
                sceneObjects = await SwitchScene(newScene);
            }
            catch (Exception ex)
            {
                sceneObjects = null;
                error = ex;
            }
            finally
            {
                Assert.Check(s_currentlyLoading.TryGetTarget(out var target) && target == this);
                s_currentlyLoading.SetTarget(null);
                _switchSceneTask = null;
            }

            if (error != null)
            {
            }
            else
            {
                Runner.RegisterSceneObjects(sceneObjects);
                Runner.InvokeSceneLoadDone();
            }
        }
    }
}