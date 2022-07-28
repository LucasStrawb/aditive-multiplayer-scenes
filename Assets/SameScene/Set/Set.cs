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
    }
}