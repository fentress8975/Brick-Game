using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientInterpolation : NetworkBehaviour
{
    [SerializeField] private NetworkTransform m_P1Bar;
    [SerializeField] private NetworkTransform m_P2Bar;
    [SerializeField] private NetworkTransform m_P1Ball;
    [SerializeField] private NetworkTransform m_P2Ball;

    [ClientRpc]
    public void SetPlayer1InterpolationClientRpc(ClientRpcParams clientRpcParams = default)
    {
        m_P2Ball.Interpolate = false;
        m_P2Bar.Interpolate = false;
    }
    [ClientRpc]
    public void SetPlayer2InterpolationClientRpc(ClientRpcParams clientRpcParams = default)
    {
        m_P1Ball.Interpolate = false;
        m_P1Bar.Interpolate = false;
    }
}
