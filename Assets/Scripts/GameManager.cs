using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private PlayersBrickHandler m_PlayersBrickHandler;
    [SerializeField] private GameUI m_GameUI;
    [SerializeField] private ulong m_Player1ID;
    [SerializeField] private ulong m_Player2ID;
    [SerializeField] private PlayerBall m_P1Ball;
    [SerializeField] private PlayerBall m_P2Ball;
    [SerializeField] private PlayerBar m_P1Bar;
    [SerializeField] private PlayerBar m_P2Bar;

    private bool[,] m_Pattern;
    private NetworkManager m_NetworkManager;

    private void Start()
    {
        if (IsHost)
        {
            m_NetworkManager = NetworkManager.Singleton;
            m_NetworkManager.OnClientDisconnectCallback += PlayerDisconnect;
            m_PlayersBrickHandler.OnPlayer1Victory += Player1Victory;
            m_PlayersBrickHandler.OnPlayer2Victory += Player2Victory;
        }
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

    public void StartGame(ulong p1, ulong p2, bool[,] pattern)
    {
        if (IsHost)
        {
            m_Player1ID = p1;
            m_Player2ID = p2;
            m_Pattern = pattern;
            m_PlayersBrickHandler.GeneratePlayerAreasSymmetrical(pattern);
            m_PlayersBrickHandler.OnPlayer1Victory += Player1Victory;
            m_PlayersBrickHandler.OnPlayer2Victory += Player2Victory;
        }
    }

    private void Player1Victory()
    {
        StopGame();
        m_GameUI.ShowVictoryScreenP1();
    }

    private void Player2Victory()
    {
        StopGame();
        m_GameUI.ShowVictoryScreenP2();
    }

    public void StopGame()
    {
        if (IsHost)
        {
            m_P1Ball.Stop();
            m_P2Ball.Stop();
            m_P1Bar.Stop();
            m_P2Bar.Stop();
        }
    }

    public void RestartGame()
    {
        if (IsHost)
        {
            m_P1Ball.ResetBall();
            m_P1Bar.ResetBar();
            m_P2Ball.ResetBall();
            m_P2Bar.ResetBar();
        }
    }

    public void EndGame()
    {
        GameNetworkHandler.Singletone.CloseGameConnection();
    }
}
