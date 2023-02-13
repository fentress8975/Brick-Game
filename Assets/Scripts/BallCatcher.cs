using UnityEngine;

public class BallCatcher : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out PlayerBall ball))
        {
            ball.ResetBall(this);
        }
    }
}
