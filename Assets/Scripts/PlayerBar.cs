using Unity.Netcode;
using UnityEngine;

public class PlayerBar : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<Direction> m_DirectionServer = new(Direction.None);
    private NetworkVariable<float> m_MaxSpeed = new(5f);
    private NetworkVariable<float> m_CurrentSpeed = new(0);
    private NetworkVariable<bool> m_isAlive = new(true);

    private void Update()
    {
        if (m_isAlive.Value && IsHost)
        {
            SpeedCheck();
            switch (m_DirectionServer.Value)
            {
                case Direction.None:
                    break;
                case Direction.Left:
                    transform.position += m_CurrentSpeed.Value * Time.deltaTime * transform.TransformDirection(Vector3.left);
                    break;
                case Direction.Right:
                    transform.position += m_CurrentSpeed.Value * Time.deltaTime * transform.TransformDirection(Vector3.right);
                    break;
                default:
                    break;
            }
        }
    }

    private void SpeedCheck()
    {
        if (m_CurrentSpeed.Value != m_MaxSpeed.Value)
        {
            m_CurrentSpeed.Value = m_MaxSpeed.Value;
        }
    }

    public void ChangeDirection(Direction direction)
    {
        m_DirectionServer.Value = direction;
    }

    public void Stop()
    {
        m_isAlive.Value = false;
        m_CurrentSpeed.Value = 0;
    }

    public void ResetBar()
    {
        m_isAlive.Value = true;
        m_CurrentSpeed.Value = m_MaxSpeed.Value;
    }
}
