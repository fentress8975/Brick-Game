using Unity.Netcode;
using UnityEngine;

public class ServerReconciliation : SingletonNetWork<ServerReconciliation>
{
    private GameNetworkHandler m_GameNetworkHandler;
    private ClientBarReconciliation m_ClientReconciliation;
    [SerializeField] private PlayerServerBar m_Player1Bar;
    [SerializeField] private PlayerServerBar m_Player2Bar;
    [SerializeField] private PlayerServerBall m_Player1Ball;
    [SerializeField] private PlayerServerBall m_Player2Ball;
    [SerializeField] private PlayersBrickClientHandler m_BricksClient;
    [SerializeField] private Material m_NoRenderMaterial;

    public void SetServerReconciliation()
    {
        if (IsHost)
        {
            HidePlayer1BarClientRpc(GameNetworkHandler.Singletone.Player1RpcParams);
            HidePlayer2BarClientRpc(GameNetworkHandler.Singletone.Player2RpcParams);
        }
    }

    [ClientRpc]
    private void HidePlayer1BarClientRpc(ClientRpcParams clientRpcParams = default)
    {
        //foreach (var item in m_BricksClient.BrickList.P1Bricks)
        //{
        //    item.gameObject.GetComponentInChildren<MeshRenderer>().material = m_NoRenderMaterial;
        //}
        //ClientBallReconciliation.Singletone.PlayerLocalBar.ResetBall();
        m_Player1Bar.gameObject.GetComponentInChildren<MeshRenderer>().material = m_NoRenderMaterial;
        m_Player1Ball.gameObject.GetComponentInChildren<MeshRenderer>().material = m_NoRenderMaterial;

    }
    [ClientRpc]
    private void HidePlayer2BarClientRpc(ClientRpcParams clientRpcParams = default)
    {
        //foreach (var item in m_BricksClient.BrickList.P2Bricks)
        //{
        //    item.gameObject.GetComponentInChildren<MeshRenderer>().material = m_NoRenderMaterial;
        //}
        //ClientBallReconciliation.Singletone.PlayerLocalBar.ResetBall();
        m_Player2Bar.gameObject.GetComponentInChildren<MeshRenderer>().material = m_NoRenderMaterial;
        m_Player2Ball.gameObject.GetComponentInChildren<MeshRenderer>().material = m_NoRenderMaterial;
    }
}
