using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField] private ServerBarControls m_ServerControls;

    private Direction m_Direction = Direction.None;
    private ClientBarReconciliation m_ClientReconciliation;

    private void Start()
    {
        m_ClientReconciliation = ClientBarReconciliation.Singletone;
    }

    private void FixedUpdate()
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
            ChangePlayerBarDirectionServerRpc(m_ClientReconciliation.AddCommandToQueue(m_Direction, m_ClientReconciliation.PlayerLocalBar.gameObject.transform.position), m_Direction);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerBarDirectionServerRpc(ulong number, Direction direction, ServerRpcParams serverRpcParams = default)
    {
        m_ServerControls.ChangePlayerBarDirectionServerRpc(number, direction, serverRpcParams.Receive.SenderClientId);
    }

}
public enum Direction
{
    Left,
    Right,
    None
}
