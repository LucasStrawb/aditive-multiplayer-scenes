using EasyButtons;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SameScene
{
    public class SetManager : NetworkBehaviour
    {
        public static SetManager Instance => _instance;
        private static SetManager _instance;

        public static int CurrentSetId => _currentSet;
        private static int _currentSet = -1;

        public static System.Action<int> OnSetChanged;

        private static readonly Dictionary<int, Set> _sets = new();

        [SerializeField] private Set _setPrefab;

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            if (Runner == null || !Runner.IsRunning || !Object.HasStateAuthority)
                return;

            var interestedSets = BasicSpawner.Players.Select(o => o.FocusSet);

            // Load
            foreach (var setId in interestedSets)
            {
                if (!_sets.TryGetValue(setId, out var set))
                    continue;

                if (set.Loaded)
                    continue;

                Debug.Log("Loading Set " + setId);
                set.Loaded = true;
            }

            // Unload
            foreach (var item in _sets)
            {
                if (interestedSets.Contains(item.Key))
                    continue;

                var set = _sets[item.Key];
                if (!set.Loaded)
                    continue;

                Debug.Log("Unloading Set " + item);
                set.Loaded = false;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out SetInputData data))
            {                
                if (data.CreateSetId > -1) // Create new Sets
                {
                    Debug.Log($"CreateSetId {data.FocusSetId}");
                    SpawnSet(data.CreateSetId);
                }
                if (data.DeleteSetId > -1) // Deelete new Sets
                {
                    Debug.Log($"DeleteSetId {data.FocusSetId}");
                    RemoveSet(data.DeleteSetId);
                }
            }
        }

        public void SpawnSet(int setId)
        {
            if (_sets.ContainsKey(setId))
                return;

            Debug.Log("Spawning Set " + setId);
            var set = Runner.Spawn(_setPrefab, new Vector3(setId * 100, 0, 0), Quaternion.identity, Runner.LocalPlayer, (_, no) =>
            {
                no.GetComponent<Set>().SetId = setId;
            });
        }

        public void DespawnSet(int setId)
        {
            if (!_sets.TryGetValue(setId, out var set))
                return;

            Debug.Log("Despawning Set " + setId);
            Runner.Despawn(set.Object);
        }

        [Button]
        public void SetCurrentSet(int value)
        {
            _currentSet = value;
            OnSetChanged?.Invoke(CurrentSetId);
        }

        internal static void AddSet(int setId, Set set)
        {
            if (!_sets.ContainsKey(setId))
                _sets.Add(setId, set);
        }

        internal static void RemoveSet(int setId)
        {
            if (_sets.ContainsKey(setId))
                _sets.Remove(setId);
        }
    }
}