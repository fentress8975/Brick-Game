using System;
using Unity.Netcode;
using UnityEngine;

public class BallCatcher : NetworkBehaviour
{
    public Action OnBallLost;
    private void OnTriggerEnter(Collider collider)
    {
        if (IsHost)
        {
            if (collider.gameObject.TryGetComponent(out PlayerServerBall ballServer))
            {
                ballServer.ResetBall();
                OnBallLost?.Invoke();
            }
        }
        if (collider.gameObject.TryGetComponent(out PlayerClientBall ballLocal))
        {
            ballLocal.ResetBall();
        }
    }
}
