using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsTracker : NetworkBehaviour
{
    [Header("Players Objects")]
    [SerializeField] private PlayersBrickHandler m_PlayersBrickHadler;
    [SerializeField] private PlayerServerBall m_Player1Ball;
    [SerializeField] private PlayerServerBall m_Player2Ball;
    [SerializeField] private BallCatcher m_Player1BallCatcher;
    [SerializeField] private BallCatcher m_Player2BallCatcher;
    [Header("Players Data")]
    private NetworkVariable<int> m_Player1BallRicochets = new();
    private NetworkVariable<int> m_Player1BallLost = new();
    private NetworkVariable<int> m_Player1BricksDestroyed = new();
    private NetworkVariable<int> m_Player2BallRicochets = new();
    private NetworkVariable<int> m_Player2BallLost = new();
    private NetworkVariable<int> m_Player2BricksDestroyed = new();
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_Player1StatsText;
    [SerializeField] private TextMeshProUGUI m_Player2StatsText;

    private void Start()
    {
        if (IsHost)
        {
            m_PlayersBrickHadler.OnPlayer1BrickDestroyed += Player1BrickDestroy;
            m_PlayersBrickHadler.OnPlayer2BrickDestroyed += Player2BrickDestroy;
            m_Player1Ball.OnBallRicochet += Player1BallRicochet;
            m_Player2Ball.OnBallRicochet += Player2BallRicochet;
            m_Player1BallCatcher.OnBallLost += Player1BallLost;
            m_Player2BallCatcher.OnBallLost += Player2BallLost;
        }
    }

    private void UpdatePlayer1Stats()
    {
        m_Player1StatsText.text = $"Ball Lost: {m_Player1BallLost.Value}\n" +
            $"Ball ricochets: {m_Player1BallRicochets.Value}\n" +
            $"Bricks destroyed: {m_Player1BricksDestroyed.Value}";
    }
    private void UpdatePlayer2Stats()
    {
        m_Player2StatsText.text = $"Ball Lost: {m_Player2BallLost.Value}\n" +
            $"Ball ricochets: {m_Player2BallRicochets.Value}\n" +
            $"Bricks destroyed: {m_Player2BricksDestroyed.Value}";
    }

    private void Player1BrickDestroy()
    {
        m_Player1BricksDestroyed.Value += 1;
        UpdateStatsClientRpc();
    }
    private void Player2BrickDestroy()
    {
        m_Player2BricksDestroyed.Value += 1;
        UpdateStatsClientRpc();
    }
    private void Player1BallLost()
    {
        m_Player1BallLost.Value += 1;
        UpdateStatsClientRpc();
    }
    private void Player2BallLost()
    {
        m_Player2BallLost.Value += 1;
        UpdateStatsClientRpc();
    }
    private void Player1BallRicochet()
    {
        m_Player1BallRicochets.Value += 1;
        UpdateStatsClientRpc();
    }
    private void Player2BallRicochet()
    {
        m_Player2BallRicochets.Value += 1;
        UpdateStatsClientRpc();
    }

    [ClientRpc]
    private void UpdateStatsClientRpc()
    {
        UpdatePlayer1Stats();
        UpdatePlayer2Stats();
    }
}
