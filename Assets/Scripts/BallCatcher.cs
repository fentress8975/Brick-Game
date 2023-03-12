using Unity.Netcode;
using UnityEngine;

public class BallCatcher : NetworkBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (IsHost)
        {
            if (collider.gameObject.TryGetComponent(out PlayerServerBall ballServer))
            {
                ballServer.ResetBall();
            }
        }
        if (collider.gameObject.TryGetComponent(out PlayerClientBall ballLocal))
        {
            ballLocal.ResetBall();
        }
    }
}
