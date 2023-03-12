using System;
using Unity.Netcode;
using UnityEngine;

public class Brick : NetworkBehaviour
{
    public Action<Brick> OnDestruction;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerServerBall _))
        {
            OnDestruction?.Invoke(this);
            OnNetworkDespawn();
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    public void Kill()
    {
        OnDestruction?.Invoke(this);
        OnNetworkDespawn();
        Destroy(gameObject);
    }
#endif
}
