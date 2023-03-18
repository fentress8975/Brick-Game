using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkHandler : SingletonNetWork<GameNetworkHandler>
{
    private const int PLAYER1 = 1;
    private const int PLAYER2 = 2;
    public string P1Name { get { return m_P1Name; } }
    public string P2Name { get { return m_P2Name; } }
    public Color P1Color { get { return m_P1Color; } }
    public Color P2Color { get { return m_P2Color; } }

    public ulong Player1ID { get { return m_Player1Id; } }
    public ulong Player2ID { get { return m_Player2Id; } }

    public PlayerServerBar Player1Bar { get { return m_P1Bar; } }
    public PlayerServerBar Player2Bar { get { return m_P2Bar; } }
    public PlayerServerBall Player1Ball { get { return m_P1Ball; } }
    public PlayerServerBall Player2Ball { get { return m_P2Ball; } }

    public ClientRpcParams Player1RpcParams { get { return m_ClientP1RpcParams; } }
    public ClientRpcParams Player2RpcParams { get { return m_ClientP2RpcParams; } }

    private NetworkManager m_NetworkManager;
    [Header("GameComponents")]
    [SerializeField] private GameManager m_GameManager;
    [SerializeField] private GameUI m_GameUI;
    [SerializeField] private ClientInterpolation m_ClientInterpolation;
    [Header("Players")]
    [SerializeField] private string m_SceneName;
    [SerializeField] private string m_P1Name = "Player1";
    [SerializeField] private string m_P2Name = "Player2";
    private Color m_P1Color;
    private Color m_P2Color;
    [SerializeField] private ulong m_Player1Id;
    [SerializeField] private ulong m_Player2Id;
    [SerializeField] private PlayerServerBar m_P1Bar;
    [SerializeField] private PlayerServerBar m_P2Bar;
    [SerializeField] private PlayerServerBall m_P1Ball;
    [SerializeField] private PlayerServerBall m_P2Ball;
    [Header("Misc")]


    private ClientRpcParams m_ClientP1RpcParams;
    private ClientRpcParams m_ClientP2RpcParams;
    private bool m_isP1ReadyToRematch = false;
    private bool m_isP2ReadyToRematch = false;
    private bool m_P2isReadyStartGame = false;
    private bool m_P1isReadyStartGame = false;


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
        m_NetworkManager = NetworkManager.Singleton;

        SetupPlayers();
        if (IsHost)
        {
            SetupPlayersID();
            LoadPlayersData();
            ClientBarReconciliation.Singletone.InitClientRpc(PLAYER1, Player1RpcParams);
            ClientBarReconciliation.Singletone.InitClientRpc(PLAYER2, Player2RpcParams);
            //ClientBallReconciliation.Singletone.InitClientRpc(PLAYER1, Player1RpcParams);
            //ClientBallReconciliation.Singletone.InitClientRpc(PLAYER2, Player2RpcParams);
            m_P1Ball.Setup(m_P1Bar, m_Player1Id);
            m_P2Ball.Setup(m_P2Bar, m_Player2Id);
            ServerReconciliation.Singletone.SetServerReconciliation();
        }
    }

    private void Update()
    {
        if (m_P1isReadyStartGame && m_P2isReadyStartGame)
        {
            EditorLogger.Log("start igry");
            m_P1isReadyStartGame = false;
            m_P2isReadyStartGame = false;
            m_GameManager.RestartGame();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void SetupPlayers()
    {
        m_GameManager.SetupPlayers(m_P1Bar, m_P2Bar, m_P1Ball, m_P2Ball);
        SetupPlayersInterpolation();
    }

    public void CloseGameConnection()
    {
        m_NetworkManager.Shutdown();
        ReturnToMainMenu();
    }

    public void TryRematchGame(ulong playerID)
    {
        if (playerID == m_Player1Id)
        {
            m_isP1ReadyToRematch = true;
        }
        if (playerID == m_Player2Id)
        {
            m_isP2ReadyToRematch = true;
        }

        if (m_isP1ReadyToRematch && m_isP2ReadyToRematch)
        {
            m_GameManager.RestartGame();
            m_GameUI.RestartGame();
            m_isP1ReadyToRematch = false;
            m_isP2ReadyToRematch = false;
        }
    }

    private void ReturnToMainMenu()
    {
        m_NetworkManager.Shutdown();
        SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
    }

    private void SetupPlayersID()
    {
        m_Player1Id = NetworkManager.Singleton.ConnectedClientsIds[0];
        m_Player2Id = NetworkManager.Singleton.ConnectedClientsIds[^1];
        EditorLogger.Log($"Player1 id {m_Player1Id} Player2 id {m_Player2Id}");
        m_ClientP1RpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { m_Player1Id }
            }
        };
        m_ClientP2RpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { m_Player2Id }
            }
        };
    }

    private void LoadPlayersData()
    {
        m_P1Name = PlayersData.Singletone.Player1Name;
        m_P2Name = PlayersData.Singletone.Player2Name;
        m_P1Color = PlayersData.Singletone.Player1Color;
        m_P2Color = PlayersData.Singletone.Player2Color;
        FixedString32Bytes p1 = m_P1Name.ToString();
        FixedString32Bytes p2 = m_P2Name.ToString();
        SendPlayersNamesClientRpc(p1, p2, Player2RpcParams);
    }

    private void SetupPlayersInterpolation()
    {
        if (IsHost)
        {
            m_ClientInterpolation.SetPlayer1InterpolationClientRpc(m_ClientP1RpcParams);
            m_ClientInterpolation.SetPlayer2InterpolationClientRpc(m_ClientP2RpcParams);
        }
    }

    internal void PlayerReadyToStartGame(ulong id)
    {
        if (id == m_Player1Id)
        {
            EditorLogger.LogError("P1 ready");
            m_P1isReadyStartGame = true;
        }
        else if (id == m_Player2Id)
        {
            EditorLogger.LogError("P2 ready");
            m_P2isReadyStartGame = true;
        }
        else
        {
            EditorLogger.LogError("Ploxo");
        }
    }

    [ClientRpc]
    private void SendPlayersNamesClientRpc(FixedString32Bytes p1Name, FixedString32Bytes p2Name, ClientRpcParams player2RpcParams)
    {
        m_P1Name = p1Name.ToString();
        m_P2Name = p2Name.ToString();
    }
}
