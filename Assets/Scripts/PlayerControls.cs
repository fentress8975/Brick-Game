using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : NetworkBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        None
    }

    private Direction m_Direction;

    void Update()
    {
        var oldDirection = m_Direction;
        m_Direction = Direction.None;

        if(Keyboard.current.leftArrowKey.wasPressedThisFrame) 
        {
            m_Direction= Direction.Left;
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            m_Direction = m_Direction == Direction.Left ? Direction.None: Direction.Right;
        }

        if(oldDirection!=m_Direction)
        {
            MoveBarServerRpc(m_Direction);
        }
    }


    [ServerRpc]
    private void MoveBarServerRpc(Direction direction)
    {

    }
}
