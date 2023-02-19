using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkHandler : SingletonNetWork<GameNetworkHandler>
{
    public string P1Name { get => m_P1Name; }
    public string P2Name { get => m_P2Name; }

    public ulong P1ID { get => m_P1ID; }
    public ulong P2ID { get => m_P2ID; }

    private NetworkManager m_NetworkManager;
    [SerializeField] private SetupNetworkPlayers m_SetupNetworkPlayers;
    [SerializeField] private GameManager m_GameManager;
    [SerializeField] private GameUI m_GameUI;

    [SerializeField] private string m_SceneName;
    [SerializeField] private string m_P1Name = "Player1";
    [SerializeField] private string m_P2Name = "Player2";
    [SerializeField] private ulong m_P1ID;
    [SerializeField] private ulong m_P2ID;

    private bool m_isP1Ready;
    private bool m_isP2Ready;


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

    private void Start()
    {
        if (IsHost)
        {
            m_NetworkManager = NetworkManager.Singleton;
            m_SetupNetworkPlayers.SetupPlayers(out m_P1ID, out m_P2ID);
            m_GameManager.SetupPlayers(m_P1ID, m_P2ID);
        }
        NetworkManager.OnClientDisconnectCallback += CloseGameConnection;
    }

    public void CloseGameConnection(ulong id = 0)
    {
        m_NetworkManager.Shutdown();
        ReturnToMainMenu();
    }

    public void TryRematchGame(ulong playerID)
    {
        if (playerID == m_P1ID)
        {
            m_isP1Ready = true;
        }
        if (playerID == m_P2ID)
        {
            m_isP2Ready = true;
        }

        if (m_isP1Ready && m_isP2Ready)
        {
            m_GameManager.RestartGame();
            m_GameUI.RestartGame();
            m_isP1Ready = false;
            m_isP2Ready = false;
        }
    }

    private void ReturnToMainMenu()
    {
        var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load {m_SceneName} " +
                  $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }
}
