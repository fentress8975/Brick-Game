using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class Wall4BarServer : NetworkBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsHost) { return; }
        if (collision.gameObject.TryGetComponent(out PlayerServerBar barServer))
        {
            //Only host allowed to change direction
            
            switch (barServer.PlayerDirection)
            {
                case Direction.Left:
                    barServer.ChangeDirectionClient(Direction.Right);
                    break;
                case Direction.Right:
                    barServer.ChangeDirectionClient(Direction.Left);
                    break;
                case Direction.None:
                    break;
            }
        }
    }
}
