using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReadyChecker : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ReadyText;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            m_ReadyText.gameObject.SetActive(false);
            PlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameNetworkHandler.Singletone.PlayerReadyToStartGame(serverRpcParams.Receive.SenderClientId);
    }
}
