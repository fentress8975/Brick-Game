using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetupNetworkPlayers : NetworkBehaviour
{
    

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
}
