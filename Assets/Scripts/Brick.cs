using System;
using Unity.Netcode;
using UnityEngine;

public class Brick : NetworkBehaviour
{
    public Action OnDestruction;

    private void OnTriggerEnter(Collider collider)
    {
        OnDestruction?.Invoke();
        OnNetworkDespawn();
        Destroy(gameObject);
    }

}
