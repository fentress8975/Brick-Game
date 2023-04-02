using UnityEngine;

public class PlayerClientBar : MonoBehaviour
{
    public Direction PlayerDirection { get => m_Direction; }
    public float PlayerCurrentSpeed { get => m_CurrentSpeed; }
    public float PlayerMaxSpeed { get => m_MaxSpeed; }
    public float PlayerPenaltyCoff { get => m_PenaltyCoff; }
    public bool isAlive { get => m_isAlive; }

    private Direction m_Direction = Direction.None;
    [SerializeField] private float m_MaxSpeed = 5f;
    [SerializeField] private float m_CurrentSpeed = 0;
    [SerializeField] private float m_PenaltyCoff = 1;
    [SerializeField] private bool m_isAlive = true;
    [SerializeField] private BarFX m_BarFX;

    private void FixedUpdate()
    {
        LocalMovement();
    }


    private void LocalMovement()
    {
        if (m_isAlive)
        {
            SpeedCheckLocal();
            switch (m_Direction)
            {
                case Direction.None:
                    break;
                case Direction.Left:
                    transform.position += m_CurrentSpeed * m_PenaltyCoff * Time.fixedDeltaTime * transform.TransformDirection(Vector3.left);
                    break;
                case Direction.Right:
                    transform.position += m_CurrentSpeed * m_PenaltyCoff * Time.fixedDeltaTime * transform.TransformDirection(Vector3.right);
                    break;
                default:
                    break;
            }
        }
    }


    public Vector3 RecalculateLocalMovement(Direction dir)
    {
        //ChangeLocalSpeed();
        switch (dir)
        {
            case Direction.None:
                break;
            case Direction.Left:
                transform.position += m_CurrentSpeed * m_PenaltyCoff * Time.fixedDeltaTime * transform.TransformDirection(Vector3.left);
                break;
            case Direction.Right:
                transform.position += m_CurrentSpeed * m_PenaltyCoff * Time.fixedDeltaTime * transform.TransformDirection(Vector3.right);
                break;
            default:
                break;
        }
        return gameObject.transform.position;
    }

    private void ChangeLocalSpeed(float currentSpeed, float maxSpeed, float penaltySpeed)
    {
        m_CurrentSpeed = currentSpeed;
        m_MaxSpeed = maxSpeed;
        m_PenaltyCoff = penaltySpeed;
    }

    private void SpeedCheckLocal()
    {
        if (m_CurrentSpeed != m_MaxSpeed)
        {
            m_CurrentSpeed = m_MaxSpeed * m_PenaltyCoff;
        }
    }

    public void ChangeDirectionLocal(Direction direction)
    {
        m_Direction = direction;
        m_BarFX.ReadMovementDirection(m_Direction);
    }
}
