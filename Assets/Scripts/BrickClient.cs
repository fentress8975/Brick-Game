using System;
using UnityEngine;


public class BrickClient : MonoBehaviour
{

    public Action<BrickClient> OnDestruction;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerClientBall _))
        {
            OnDestruction?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
