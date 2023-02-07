using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyBehaviour : NetworkBehaviour
{
    [SerializeField] private TMP_InputField m_PlayerNickNameInputField;

    [SerializeField] private TextMeshProUGUI m_Player1NickName;
    [SerializeField] private TextMeshProUGUI m_Player2NickName;
    [SerializeField] private Button m_StartGameButton;

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
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {m_SceneName} " +
                          $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        EditorLogger.Log("OnNetworkSpawn");
        m_Player1Name.OnValueChanged += (oldValue, newValue) =>
        {
            m_Player1NickName.text = newValue.ToString();
        };
        m_Player2Name.OnValueChanged += (oldValue, newValue) =>
        {
            m_Player2NickName.text = newValue.ToString();
        };
        if (IsHost)
        {
            m_Player1Name.Value = m_PlayerNickNameInputField.text;
        }
        else if (IsClient)
        {
            m_Player1NickName.text = m_Player1Name.Value.ToString();
            SendPlayerName(m_PlayerNickNameInputField.text);
        }
    }

    private void ClientConnected(ulong id)
    {
        EditorLogger.Log("Lobby connected " + id);
    }

    private void SendPlayerName(string name)
    {
        SendPlayerNameServerRpc(new(name));
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendPlayerNameServerRpc(FixedString32Bytes name)
    {
        m_Player2Name.Value = name;
    }

    [ClientRpc]
    private void SendHostNameClientRpc(FixedString32Bytes name)
    {
        m_Player1NickName.text = name.ToString();
    }
}
