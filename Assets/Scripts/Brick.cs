using Unity.Netcode;
using UnityEngine;

public class Brick : NetworkBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        EditorLogger.Log("test");
        OnNetworkDespawn();
        Destroy(gameObject);
    }

}
