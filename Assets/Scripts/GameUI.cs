using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour
{

    [SerializeField] private Button m_DisconnectButton;
    [SerializeField] private Button m_EndGameButton;
    [SerializeField] private Button m_RematchButton;
    [SerializeField] private Button m_SurrenderButton;
    [SerializeField] private TextMeshProUGUI m_VictoryPlayerNickname;

    [SerializeField] private GameObject m_MenuPanel;
    [SerializeField] private GameObject m_VictoryPanel;

    [SerializeField] private TextMeshProUGUI m_P1Nickname;
    [SerializeField] private TextMeshProUGUI m_P2Nickname;

    private void Start()
    {
        m_MenuPanel.SetActive(false);
        m_VictoryPanel.SetActive(false);

        m_DisconnectButton.onClick.AddListener(Disconnect);
        m_EndGameButton.onClick.AddListener(Disconnect);
        m_RematchButton.onClick.AddListener(RematchGame);
        m_SurrenderButton.onClick.AddListener(Surrender);

        SetPlayersNicknamesClientRpc();
    }

    private void Surrender()
    {
        //add cool stuff pls
        GameNetworkHandler.Singletone.CloseGameConnection();
    }

    private void RematchGame()
    {
        RematchGameServerRpc();
    }

    private void Disconnect()
    {
        GameNetworkHandler.Singletone.CloseGameConnection();
    }

    public void ShowVictoryScreenP1()
    {
        ShowVictoryScreenP1ClientRpc();
    }

    public void ShowVictoryScreenP2()
    {
        ShowVictoryScreenP2ClientRpc();
    }

    public void RestartGame()
    {
        RestartGameClientRpc();
    }

    private void SetPlayersNicknames(FixedString32Bytes p1, FixedString32Bytes p2)
    {
        m_P1Nickname.text = p1.ToString();
        m_P2Nickname.text = p2.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RematchGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameNetworkHandler.Singletone.TryRematchGame(serverRpcParams.Receive.SenderClientId);
    }
    [ClientRpc]
    private void RestartGameClientRpc()
    {
        m_MenuPanel.SetActive(false);
        m_VictoryPanel.SetActive(false);
    }
    [ClientRpc]
    private void ShowVictoryScreenP2ClientRpc()
    {
        m_VictoryPlayerNickname.text = $"Congrats {GameNetworkHandler.Singletone.P2Name}";
        m_VictoryPanel.SetActive(true);
    }
    [ClientRpc]
    private void ShowVictoryScreenP1ClientRpc()
    {
        m_VictoryPlayerNickname.text = $"Congrats {GameNetworkHandler.Singletone.P1Name}";
        m_VictoryPanel.SetActive(true);
    }
    [ClientRpc]
    private void SetPlayersNicknamesClientRpc()
    {
        SetPlayersNicknames(new(PlayerPrefs.GetString("Player1Nickname")),
                            new(PlayerPrefs.GetString("Player2Nickname")));
    }
}
