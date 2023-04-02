using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private Button m_DisconnectButton;
    [SerializeField] private Button m_SurrenderButton;
    [SerializeField] private GameObject m_MenuPanel;
    [Header("Victory Panels UI")]
    [SerializeField] private Canvas m_VictoryCanvas;
    [SerializeField] private TextMeshProUGUI m_P1VictoryPlayerNickname;
    [SerializeField] private TextMeshProUGUI m_P2VictoryPlayerNickname;
    [SerializeField] private List<Button> m_EndGameButtons;
    [SerializeField] private List<Button> m_RematchButtons;
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI m_P1Nickname;
    [SerializeField] private TextMeshProUGUI m_P2Nickname;

    [SerializeField] private Sprite m_BadConnectionSprite;
    [SerializeField] private Sprite m_MediumConnectionSprite;
    [SerializeField] private Sprite m_GoodConnectionSprite;
    [SerializeField] private Image m_P1ConnectionStatus;
    [SerializeField] private Image m_P2ConnectionStatus;



    private void Start()
    {
        m_MenuPanel.SetActive(false);
        SetActiveWinPanels(false);
        StartCoroutine(PlayersRTTCoroutine());

        m_DisconnectButton.onClick.AddListener(Disconnect);
        foreach (var button in m_EndGameButtons)
        {
            button.onClick.AddListener(Disconnect);
        }
        foreach (var button in m_RematchButtons)
        {
            button.onClick.AddListener(RematchGame);
        }
        m_SurrenderButton.onClick.AddListener(Surrender);
        if (IsHost)
        {
            GetPlayersNicknamesServerRpc();
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            m_MenuPanel.gameObject.SetActive(!m_MenuPanel.gameObject.activeInHierarchy);
        }

        UpdateConnectionStatus();
    }

    private void SetActiveWinPanels(bool active)
    {
        m_VictoryCanvas.gameObject.SetActive(active);
    }

    private IEnumerator PlayersRTTCoroutine()
    {
        while (true)
        {
            UpdateConnectionStatus();
            yield return new WaitForSeconds(3f);
        }
    }

    private void UpdateConnectionStatus()
    {
        int ping = GameNetworkHandler.Singletone.PlayerPing;
        if (GameNetworkHandler.Singletone.NumberPlayer == GameNetworkHandler.PlayerNumber.One)
        {
            if (ping < 50)
            {
                m_P1ConnectionStatus.sprite = m_GoodConnectionSprite;
            }
            else if (ping < 100)
            {
                m_P1ConnectionStatus.sprite = m_MediumConnectionSprite;
            }
            else
            {
                m_P1ConnectionStatus.sprite = m_BadConnectionSprite;
            }
        }
        else
        {
            if (ping < 50)
            {
                m_P2ConnectionStatus.sprite = m_GoodConnectionSprite;
            }
            else if (ping < 100)
            {
                m_P2ConnectionStatus.sprite = m_MediumConnectionSprite;
            }
            else
            {
                m_P2ConnectionStatus.sprite = m_BadConnectionSprite;
            }
        }
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
        m_P1VictoryPlayerNickname.text = $"Congrats {GameNetworkHandler.Singletone.P1Name}";
        m_P2VictoryPlayerNickname.text = $"Unlucky {GameNetworkHandler.Singletone.P2Name}";
        SetActiveWinPanels(true);
    }

    public void ShowVictoryScreenP2()
    {
        m_P1VictoryPlayerNickname.text = $"Unlucky {GameNetworkHandler.Singletone.P1Name}";
        m_P2VictoryPlayerNickname.text = $"Congrats {GameNetworkHandler.Singletone.P2Name}";
        SetActiveWinPanels(true);

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
        SetActiveWinPanels(false);
    }
    [ClientRpc]
    public void ShowVictoryScreenP2ClientRpc()
    {
        m_P1VictoryPlayerNickname.text = $"What a shame {GameNetworkHandler.Singletone.P1Name}";
        m_P2VictoryPlayerNickname.text = $"Congrats {GameNetworkHandler.Singletone.P2Name}";
        SetActiveWinPanels(true);

    }
    [ClientRpc]
    public void ShowVictoryScreenP1ClientRpc()
    {
        m_P1VictoryPlayerNickname.text = $"Congrats {GameNetworkHandler.Singletone.P1Name}";
        m_P2VictoryPlayerNickname.text = $"Unlucky {GameNetworkHandler.Singletone.P2Name}";
        SetActiveWinPanels(true);
    }
    [ClientRpc]
    private void SetPlayersNicknamesClientRpc(FixedString32Bytes p1, FixedString32Bytes p2)
    {
        SetPlayersNicknames(p1, p2);
    }
    [ServerRpc]
    private void GetPlayersNicknamesServerRpc()
    {
        SetPlayersNicknamesClientRpc(new(PlayerPrefs.GetString("Player1Nickname")),
                                     new(PlayerPrefs.GetString("Player2Nickname")));
    }
}
