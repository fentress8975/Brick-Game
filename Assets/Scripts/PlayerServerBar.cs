using Unity.Netcode;
using UnityEngine;

public class PlayerServerBar : NetworkBehaviour
{
    public float PlayerCurrentSpeed { get => m_CurrentSpeed.Value; }
    public float PlayerMaxSpeed { get => m_MaxSpeed.Value; }
    public float PlayerPenaltyCoff { get => m_PenaltyCoff.Value; }
    public bool isAlive { get => m_isAlive.Value; }


    [Header("Server Variables")]
    [SerializeField] private NetworkVariable<Direction> m_Direction = new(Direction.None);
    [SerializeField] private NetworkVariable<float> m_MaxSpeed = new(5f);
    [SerializeField] private NetworkVariable<float> m_CurrentSpeed = new(0);
    [SerializeField] private NetworkVariable<float> m_PenaltyCoff = new(1);
    [SerializeField] private NetworkVariable<bool> m_isAlive = new(true);

    private void FixedUpdate()
    {
        NetworkMovement();
    }

    private void NetworkMovement()
    {
        if (m_isAlive.Value && IsHost)
        {
            SpeedCheckClient();
            switch (m_Direction.Value)
            {
                case Direction.None:
                    break;
                case Direction.Left:
                    transform.position += m_CurrentSpeed.Value * m_PenaltyCoff.Value * Time.fixedDeltaTime * transform.TransformDirection(Vector3.left);
                    break;
                case Direction.Right:
                    transform.position += m_CurrentSpeed.Value * m_PenaltyCoff.Value * Time.fixedDeltaTime * transform.TransformDirection(Vector3.right);
                    break;
                default:
                    break;
            }
        }
    }

    private void SpeedCheckClient()
    {
        if (m_CurrentSpeed.Value != m_MaxSpeed.Value)
        {
            m_CurrentSpeed.Value = m_MaxSpeed.Value * m_PenaltyCoff.Value;
        }
    }

    public void ChangeDirectionClient(Direction direction)
    {
        m_Direction.Value = direction;
    }

    public void StopClient()
    {
        m_isAlive.Value = false;
        m_CurrentSpeed.Value = 0;
    }

    public void ResetBarClient()
    {
        m_isAlive.Value = true;
        m_CurrentSpeed.Value = m_MaxSpeed.Value;
    }
}
