using Unity.Netcode;
using UnityEngine;

public class SetupNetworkPlayers : NetworkBehaviour
{
    [SerializeField] private PlayerControls m_PlayerControls;
    [SerializeField] private ServerBarControls m_ServerControls;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (x) =>
        {
            if (NetworkManager.Singleton.ConnectedClientsIds.Count > 1) { ConfigPlayers(); }
        };

        if (IsHost)
        {
            ConfigPlayers();
        }
    }


    [ContextMenu("Force Config Players")]
    private void ConfigPlayers()
    {
        m_ServerControls.SetupPlayers(NetworkManager.Singleton.ConnectedClientsIds[0], NetworkManager.Singleton.ConnectedClientsIds[^1]);
    }
}
