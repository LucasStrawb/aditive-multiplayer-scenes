using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Test
{
    public static class ExtensionHelper
    {
        private static MyCustomNetworkSceneManager _sceneManager;

        public static MyCustomNetworkSceneManager SceneManager(this NetworkRunner _) => _sceneManager;
        public static void SetSceneManager(this NetworkRunner _, MyCustomNetworkSceneManager manager) => _sceneManager = manager;

        public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation op)
        {
            var task = new TaskCompletionSource<AsyncOperation>();
            op.completed += (operation) => task.SetResult(op);
            return task.Task.GetAwaiter();
        }
    }
}