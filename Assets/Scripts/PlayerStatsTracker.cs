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
    [Header("UI debug")]
    [SerializeField] private TextMeshProUGUI m_P1BallLost;
    [SerializeField] private TextMeshProUGUI m_P1BallRicochets;
    [SerializeField] private TextMeshProUGUI m_P1BricksDestroyed;
    [SerializeField] private TextMeshProUGUI m_P21BallLost;
    [SerializeField] private TextMeshProUGUI m_P2BallRicochets;
    [SerializeField] private TextMeshProUGUI m_P2BricksDestroyed;

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

    private void Player1BrickDestroy()
    {

        m_Player1BricksDestroyed.Value += 1;
        m_P1BricksDestroyed.text = m_Player1BricksDestroyed.Value.ToString();
    }
    private void Player2BrickDestroy()
    {
        m_Player2BricksDestroyed.Value += 1;
        m_P2BricksDestroyed.text = m_Player2BricksDestroyed.Value.ToString();

    }
    private void Player1BallLost()
    {
        m_Player1BallLost.Value += 1;
    }
    private void Player2BallLost()
    {
        m_Player2BallLost.Value += 1;
    }
    private void Player1BallRicochet()
    {
        m_Player1BallRicochets.Value += 1;
    }
    private void Player2BallRicochet()
    {
        m_Player2BallRicochets.Value += 1;

    }
}
