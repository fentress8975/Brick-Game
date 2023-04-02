using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Wall4Bar : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerClientBar barClient))
        {
            switch (barClient.PlayerDirection)
            {
                case Direction.Left:
                    barClient.ChangeDirectionLocal(Direction.Right);
                    break;
                case Direction.Right:
                    barClient.ChangeDirectionLocal(Direction.Left);
                    break;
                case Direction.None:
                    break;
            }
        }
    }
}
