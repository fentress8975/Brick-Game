using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyBehaviour : NetworkBehaviour
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif
    [SerializeField] private MainMenuUI m_MainMenuUI;
    [SerializeField] private TMP_InputField m_PlayerNickNameInputField;
    [SerializeField] private TextMeshProUGUI m_Player1NickName;
    [SerializeField] private TextMeshProUGUI m_Player2NickName;
    [SerializeField] private TextMeshProUGUI m_CountdownText;
    [SerializeField] private Button m_StartGameButton;
    [SerializeField] private Button m_Player1ReadyButton;
    [SerializeField] private Button m_Player2ReadyButton;
    private ulong m_Player1ID = new();
    private ulong m_Player2ID = new();

    private NetworkVariable<bool> m_isPlayer1Ready = new(false);
    private NetworkVariable<bool> m_isPlayer2Ready = new(false);
    private bool m_isGameStarting = false;

    [SerializeField] private string m_SceneName;

    private NetworkVariable<FixedString32Bytes> m_Player1Name = new();
    private NetworkVariable<FixedString32Bytes> m_Player2Name = new();

    private NetworkManager m_NetworkManager;

    private void Start()
    {
        m_NetworkManager = NetworkManager.Singleton.GetComponent<NetworkManager>();
        if (m_NetworkManager != null)
        {
            m_NetworkManager.OnClientConnectedCallback += ClientConnected;
        }
        m_StartGameButton.onClick.AddListener(() => { LoadGameScene(); });
        m_Player1ReadyButton.onClick.AddListener(() => { Player1ReadyServerRpc(); });
        m_Player2ReadyButton.onClick.AddListener(() => { Player2ReadyServerRpc(); });
        m_NetworkManager.OnClientDisconnectCallback += Disconnect;
        SetupPlayersButtons();

        void SetupPlayersButtons()
        {
            m_isPlayer1Ready.OnValueChanged += (bool oldVal, bool newVal) =>
            {
                if (newVal == true)
                {
                    m_Player1ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player1 Ready";
                }
                else
                {
                    m_Player1ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player1 not ready";

                }
            };
            m_isPlayer2Ready.OnValueChanged += (bool oldVal, bool newVal) =>
            {
                if (newVal == true)
                {
                    m_Player2ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player2 Ready";
                }
                else
                {
                    m_Player2ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player2 not ready";

                }
            };
        }

    }

    private void Disconnect(ulong id)
    {
        if (IsHost)
        {
            if (id == m_Player1ID)
            {
                m_MainMenuUI.ReturnToMainMenu();
            }
            else
            {
                m_Player2Name.Value = "WaitForPlayer";
            }
        }
        else 
        { 
            m_MainMenuUI.ReturnToMainMenu();
            NetworkSetup.Singletone.Disconnect();
        }
    }

    private void Update()
    {
        if (IsHost)
        {
            if (m_isPlayer1Ready.Value && m_isPlayer2Ready.Value && m_isGameStarting == false)
            {
                m_isGameStarting = true;
                StartCoroutine(StartCountdown());
            }
            else if (m_isGameStarting && (m_isPlayer1Ready.Value == false || m_isPlayer2Ready.Value == false))
            {
                StopAllCoroutines();
                m_isGameStarting = false;
                m_CountdownText.enabled = false;
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        int timer = 3;
        m_CountdownText.enabled = true;
        while (timer > 0)
        {
            UpdateCountDownClientRpc(timer);
            timer--;
            yield return new WaitForSeconds(1);
        }
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        if (m_NetworkManager.ConnectedClientsIds.Count != 2)
        {
            EditorLogger.Log("Нужно 2 игрока");
            return;
        }
        else
        {
            if (IsHost && !string.IsNullOrEmpty(m_SceneName))
            {
                SavePlayersName();
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {m_SceneName} " +
                          $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }
    }

    private void SavePlayersName()
    {
        PlayerPrefs.SetString("Player1Nickname", m_Player1NickName.text);
        PlayerPrefs.SetString("Player2Nickname", m_Player2NickName.text);
    }

    public override void OnNetworkSpawn()
    {
        SetUpdatingNickNames();
        if (IsHost)
        {
            m_Player1Name.Value = m_PlayerNickNameInputField.text;
            m_Player2Name.Value = "WaitForPlayer";
        }
        else if (IsClient)
        {
            m_Player1NickName.text = m_Player1Name.Value.ToString();
            SendPlayerNameServerRpc(new FixedString32Bytes(m_PlayerNickNameInputField.text));
            m_StartGameButton.enabled = false;
        }

        void SetUpdatingNickNames()
        {
            m_Player1Name.OnValueChanged += (oldValue, newValue) =>
            {
                m_Player1NickName.text = newValue.ToString();
            };
            m_Player2Name.OnValueChanged += (oldValue, newValue) =>
            {
                m_Player2NickName.text = newValue.ToString();
            };
        }
    }

    private void SetPlayersID()
    {
        if (IsHost)
        {
            m_Player1ID = NetworkManager.Singleton.ConnectedClientsIds[0];
            m_Player2ID = NetworkManager.Singleton.ConnectedClientsIds[^1];
        }
    }

    private void ClientConnected(ulong id)
    {
        EditorLogger.Log("Lobby connected " + id);
        SetPlayersID();
    }

    [ServerRpc(RequireOwnership = false)]
    private void Player1ReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId == m_Player1ID)
        {
            m_isPlayer1Ready.Value = m_isPlayer1Ready.Value != true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void Player2ReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId == m_Player2ID)
        {
            m_isPlayer2Ready.Value = m_isPlayer2Ready.Value != true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendPlayerNameServerRpc(FixedString32Bytes name)
    {
        m_Player2Name.Value = name;
    }

    [ClientRpc]
    private void UpdateCountDownClientRpc(int time)
    {
        m_CountdownText.text = $"Game starting in {time}";
    }
}
