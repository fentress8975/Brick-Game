using System;
using Unity.Netcode;
using UnityEngine;

public class Brick : NetworkBehaviour
{
    public Action OnDestruction;

    private void OnCollisionEnter(Collision collision)
    {
        OnDestruction?.Invoke();
        OnNetworkDespawn();
        Destroy(gameObject);
    }

    public void Destroy(PlayerBall caller)
    {
        OnDestruction?.Invoke();
        OnNetworkDespawn();
        Destroy(gameObject);
    }
}
