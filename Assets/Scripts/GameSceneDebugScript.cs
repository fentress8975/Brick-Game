using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSceneDebugScript : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI m_RTTText;
    [SerializeField] private TextMeshProUGUI m_ReadyText;

    private void Start()
    {
        StartCoroutine(PlayersRTTCoroutine());
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            m_ReadyText.gameObject.SetActive(false);
            PlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameNetworkHandler.Singletone.PlayerReadyToStartGame(serverRpcParams.Receive.SenderClientId);
    }

    private IEnumerator PlayersRTTCoroutine()
    {
        while (true)
        {
            CalculatePlayerRTT();
            yield return new WaitForSeconds(1f);
        }
    }

    private void CalculatePlayerRTT()
    {
        float RTT = NetworkManager.LocalTime.TimeAsFloat - NetworkManager.ServerTime.TimeAsFloat;
        m_RTTText.text = $"Ping: {RTT} мс";
    }
}
