using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Local;

    [Networked, Capacity(10)]
    public NetworkLinkedList<int> Sets => default;

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
        if (GetInput(out NetworkInputData data))
        {
            if (data.SetId == -1)
                return;

            if (Sets.Contains(data.SetId))
            {
                Sets.Remove(data.SetId);
                Debug.Log("Removing set " + data.SetId);
            }
            else
            {
                Sets.Add(data.SetId);
                Debug.Log("Adding set " + data.SetId);
            }
        }
    }

}
