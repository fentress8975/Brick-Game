using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private PlayersBrickHandler m_PlayersBrickHandler;
    [SerializeField] private GameUI m_GameUI;
    [SerializeField] private ulong m_Player1ID;
    [SerializeField] private ulong m_Player2ID;
    [SerializeField] private PlayerServerBall m_P1Ball;
    [SerializeField] private PlayerServerBall m_P2Ball;
    [SerializeField] private PlayerServerBar m_P1Bar;
    [SerializeField] private PlayerServerBar m_P2Bar;

    [SerializeField] private bool[,] m_Pattern;
    private NetworkManager m_NetworkManager;
    private GameNetworkHandler m_GameNetworkHandler;

    private void Start()
    {
        if (IsHost)
        {
            m_NetworkManager = NetworkManager.Singleton;
            m_NetworkManager.OnClientDisconnectCallback += PlayerDisconnect;
        }
        m_GameNetworkHandler = GameNetworkHandler.Singletone;
    }

    private void PlayerDisconnect(ulong id)
    {
        if (IsHost)
        {
            if (id == m_Player1ID)
            {
                Player2Victory();
            }
            else if (id == m_Player2ID)
            {
                Player1Victory();
            }
            else
            {
                throw new Exception("ќтключение не существующего игрока");
            }
        }
    }

    public void SetupPlayers(PlayerServerBar p1Bar, PlayerServerBar p2Bar, PlayerServerBall p1Ball, PlayerServerBall p2Ball)
    {
        if (IsHost)
        {
            m_Player1ID = m_GameNetworkHandler.Player1ID;
            m_Player2ID = m_GameNetworkHandler.Player2ID;
            m_P1Bar = p1Bar;
            m_P2Bar = p2Bar;
            m_P1Ball = p1Ball;
            m_P2Ball = p2Ball;
            m_PlayersBrickHandler.OnPlayer1Victory += Player1Victory;
            m_PlayersBrickHandler.OnPlayer2Victory += Player2Victory;

            m_P1Ball.Stop();
            m_P2Ball.Stop();
        }
    }

    private void StartGame()
    {
        ResetPlayersObjects();
        m_PlayersBrickHandler.GeneratePlayerAreasSymmetrical();
    }

    private void Player1Victory()
    {
        EditorLogger.Log("P1 win");
        StopGame();
        m_GameUI.ShowVictoryScreenP1();
    }

    private void Player2Victory()
    {
        EditorLogger.Log("P2 win");
        StopGame();
        m_GameUI.ShowVictoryScreenP2();
    }

    public void StopGame()
    {
        if (IsHost)
        {
            m_P1Ball.Stop();
            m_P2Ball.Stop();
            m_P1Bar.StopClient();
            m_P2Bar.StopClient();
        }
    }

    public void RestartGame()
    {
        if (IsHost)
        {
            ResetPlayersObjects();
            StartGame();
        }
    }

    private void ResetPlayersObjects()
    {
        m_P1Ball.ResetBall();
        m_P1Bar.ResetBarClient();
        m_P2Ball.ResetBall();
        m_P2Bar.ResetBarClient();
    }

    public void EndGame()
    {
        GameNetworkHandler.Singletone.CloseGameConnection();
    }

#if UNITY_EDITOR
    public void DebbugPattern(bool[,] pattern)
    {
        m_Pattern = pattern;
    }
#endif
}
