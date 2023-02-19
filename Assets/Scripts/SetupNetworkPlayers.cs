using Unity.Netcode;
using UnityEngine;

public class SetupNetworkPlayers : NetworkBehaviour
{
    [SerializeField] private PlayerControls m_PlayerControls;
    [SerializeField] private ServerBarControls m_ServerControls;

    private ulong m_Player1ID;
    private ulong m_Player2ID;


    public void SetupPlayers(out ulong p1ID, out ulong p2ID)
    {
        ConfigPlayers();

        p1ID = m_Player1ID;
        p2ID = m_Player2ID;
    }


    [ContextMenu("Force Config Players")]
    private void ConfigPlayers()
    {
        m_Player1ID = NetworkManager.Singleton.ConnectedClientsIds[0];
        m_Player2ID = NetworkManager.Singleton.ConnectedClientsIds[^1];
        m_ServerControls.SetupPlayers(m_Player1ID, m_Player2ID);
    }
}
