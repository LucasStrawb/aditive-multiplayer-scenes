using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SameScene
{
    public class Set : NetworkBehaviour
    {
        [Networked]
        public int SetId { get; set; } = -1;

        public SetData SetData;

        public bool Loaded
        {
            get
            {
                return _loaded;
            }
            set
            {
                _loaded = value;
                if (value)
                    SpawnMocObjects();
                else
                    DespawnObjects();
            }
        }
        private bool _loaded;

        private List<NetworkObject> _loadedObjects = new();

        public override void Spawned()
        {
            base.Spawned();
            SetManager.AddSet(SetId, this);
            gameObject.name = "Set " + SetId;
            Debug.Log($"Set {SetId} Spawned");
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            SetManager.RemoveSet(SetId);
            Debug.Log($"Set {SetId} Despawned");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetColor(SetId);
            Gizmos.DrawWireCube(transform.position + Vector3.up * transform.localScale.y / 2, transform.localScale);
        }

        public void SpawnObjects()
        {
            // TODO spawn objects from network
            SpawnMocObjects();
        }

        [ContextMenu("SpawnMocObjects")]
        private void SpawnMocObjects()
        {
            foreach (var item in SetData.GameObjects)
            {
                SpawnObject(item);
            }
        }

        private void SpawnObject(GameObject gameObject)
        {
            var networkObject = Runner.Spawn(gameObject, gameObject.transform.position + transform.position, null, null, (_, networkObject) =>
            {
                var renderer = networkObject.gameObject.AddComponent<SetObjectRenderer>();
                renderer.SetSetId(SetId);
            });
            _loadedObjects.Add(networkObject);
        }

        private void DespawnObjects()
        {
            var loadedObjects = new List<NetworkObject>(_loadedObjects);
            foreach (var networkObject in loadedObjects)
            {
                DespawnObject(networkObject);
            }
        }

        private void DespawnObject(NetworkObject networkObject)
        {
            Runner.Despawn(networkObject);
            _loadedObjects.Remove(networkObject);
        }

        public static Color GetColor(int id)
        {
            if (id < 0)
                return Color.white;

            var colors = new[]
            {
                Color.blue,
                Color.cyan,
                Color.green,
                Color.yellow,
                Color.red,
                Color.magenta,
            };
            var remaning = id % colors.Length;
            return colors[remaning];
        }
    }
}