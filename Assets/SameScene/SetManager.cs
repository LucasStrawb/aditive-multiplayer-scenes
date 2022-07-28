using EasyButtons;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SameScene
{
    public class SetManager : NetworkBehaviour
    {
        public static int CurrentSet => _currentSet;
        private static int _currentSet = -1;

        public static System.Action<int> OnSetChanged;

        private static Dictionary<int, Set> _loadedSets = new();

        [SerializeField] private Set _setPrefab;

        private void Update()
        {
            if (Runner == null || !Runner.IsRunning || !Object.HasStateAuthority)
                return;

            var interestedSets = BasicSpawner.Players.SelectMany(o => o.Sets);

            // Load
            foreach (var setId in interestedSets)
            {
                if (_loadedSets.ContainsKey(setId))
                    continue;

                Debug.Log("Spawning Set " + setId);
                var set = Runner.Spawn(_setPrefab, new Vector3(setId, 0, 0), Quaternion.identity, Runner.LocalPlayer, (_, no) =>
                {
                    no.GetComponent<Set>().SetId = setId;
                });
            }

            // Unload
            var loadedSets = new Dictionary<int, Set>(_loadedSets);
            foreach (var setId in loadedSets)
            {
                if (interestedSets.Contains(setId.Key))
                    continue;

                Debug.Log("Despawning Set " + setId);
                Runner.Despawn(setId.Value.Object);
            }
        }

        [Button]
        public void SetCurrentSet(int value)
        {
            _currentSet = value;
            OnSetChanged?.Invoke(CurrentSet);
        }

        public static void AddSet(int setId, Set set)
        {
            if (!_loadedSets.ContainsKey(setId))
                _loadedSets.Add(setId, set);
        }

        internal static void RemoveSet(int setId)
        {
            if (_loadedSets.ContainsKey(setId))
                _loadedSets.Remove(setId);
        }
    }
}