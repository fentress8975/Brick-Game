using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField] private ServerBarControls m_ServerControls;

    private Direction m_Direction;

    private void Update()
    {
        var oldDirection = m_Direction;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                m_Direction = Direction.None;
            }
            else
            {
                m_Direction = Direction.Left;
            }
        }
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            m_Direction = Direction.Right;
        }
        else
        {
            m_Direction = Direction.None;
        }

        if (oldDirection != m_Direction)
        {
            ChangePlayerBarDIrectionServerRpc(m_Direction);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void ChangePlayerBarDIrectionServerRpc(Direction direction, ServerRpcParams serverRpcParams = default)
    {
        m_ServerControls.ChangePlayerBarDirectionServerRpc(direction, serverRpcParams.Receive.SenderClientId);

    }

}
public enum Direction
{
    Left,
    Right,
    None
}
