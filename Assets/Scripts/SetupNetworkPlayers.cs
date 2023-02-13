using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetupNetworkPlayers : NetworkBehaviour
{
    [SerializeField] private PlayerControls m_PlayerControls;
    [SerializeField] private ServerBarControls m_ServerControls;


    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            //give player1
            return;
        }
        if(IsClient)
        {
            //give player2
            return;
            
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (x) =>
        {
            if (NetworkManager.Singleton.ConnectedClientsIds.Count > 1) { ConfigPlayers(); }
        };

        ConfigPlayers();
    }


    [ContextMenu("Debug force config Players")]
    private void ConfigPlayers()
    {
        m_ServerControls.SetupPlayers(NetworkManager.Singleton.ConnectedClientsIds[0], NetworkManager.Singleton.ConnectedClientsIds[^1]);
    }
}
