using Unity.Netcode;
using UnityEngine;

public class BallCatcher : NetworkBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (IsHost)
        {
            if (collider.gameObject.TryGetComponent(out PlayerBall ball))
            {
                ball.ResetBall();
            }
        }
    }
}
