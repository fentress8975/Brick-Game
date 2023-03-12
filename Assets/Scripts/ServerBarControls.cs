using UnityEngine;

public class ServerBarControls : MonoBehaviour
{
    [SerializeField] private PlayerServerBar m_Player1ServerBar;
    [SerializeField] private PlayerServerBar m_Player2ServerBar;

    public void ChangePlayerBarDirectionServerRpc(ulong number, Direction direction, ulong id)
    {
        ulong clientId = id;
        if (clientId == GameNetworkHandler.Singletone.Player1ID)
        {
            m_Player1ServerBar.ChangeDirectionClient(direction);
            ClientBarReconciliation.Singletone.ReturnCompletedCommandClientRpc(number, m_Player1ServerBar.gameObject.transform.position, GameNetworkHandler.Singletone.Player1RpcParams);
        }
        else if (clientId == GameNetworkHandler.Singletone.Player2ID)
        {
            m_Player2ServerBar.ChangeDirectionClient(direction);
            ClientBarReconciliation.Singletone.ReturnCompletedCommandClientRpc(number, m_Player2ServerBar.gameObject.transform.position, GameNetworkHandler.Singletone.Player2RpcParams);
        }
        else
        {
            EditorLogger.Log("Vse ochen ploxo");
        }
    }

}
