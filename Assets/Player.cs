using Fusion;
using SameScene;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Local;

    public Action<int> OnSetChanged;

    [Networked]
    public int FocusSet { get; set; } = -1;

    private void Update()
    {
        if (Object != null && Local == null && Object.HasInputAuthority)
        {
            Local = this;
            Object.RequestStateAuthority();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out SetInputData data))
        {
            if (data.FocusSetId > -1)
            {
                FocusSet = data.FocusSetId;
                OnSetChanged?.Invoke(data.FocusSetId);
                Debug.Log($"Player {gameObject.name} focus set as {data.FocusSetId}");
            }

            if (data.CreateSetId > -1) // Create new Sets
            {
                Debug.Log($"CreateSetId {data.FocusSetId}");
                SetManager.Instance.SpawnSet(data.CreateSetId);
            }

            if (data.DeleteSetId > -1) // Deelete new Sets
            {
                Debug.Log($"DeleteSetId {data.FocusSetId}");
                SetManager.Instance.DespawnSet(data.DeleteSetId);
            }
        }
    }
}
