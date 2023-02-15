using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerBall : NetworkBehaviour
{
    public PlayerBar Bar { get => m_Bar; }

    [SerializeField] private NetworkVariable<float> m_BallSpeed = new(0);
    [SerializeField] private NetworkVariable<float> m_MaxSpeed = new(5);
    [SerializeField] private NetworkVariable<float> m_Penalty = new(1.5f);
    [SerializeField] private NetworkVariable<ulong> m_PlayerId = new();

    private Rigidbody m_Rigidbody;
    [SerializeField] private PlayerBar m_Bar;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(PlayerBar playerBar, ulong id)
    {

    }

    private void FixedUpdate()
    {
        if (m_BallSpeed.Value <= 0)
        {
            return;
        }
        Move();
    }

    private void Move()
    {
        m_Rigidbody.MovePosition(transform.position + m_BallSpeed.Value * Time.fixedDeltaTime * transform.TransformDirection(Vector3.forward));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider _collider = collision.collider;
        Vector3 _collisionPoint = _collider.ClosestPointOnBounds(transform.position) + transform.TransformDirection(Vector3.back * 5);
        Ray _directionRay = new(_collisionPoint, transform.TransformDirection(Vector3.forward));
        if (_collider.Raycast(_directionRay, out RaycastHit hitInfo, Mathf.Infinity))
        {
            m_Rigidbody.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.TransformDirection(Vector3.forward), hitInfo.normal), Vector3.back);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (collision.gameObject.TryGetComponent(out Brick brick))
    //    {
    //        brick.Destroy(this);
    //    }
    //}

    public void ResetBall()
    {
        if (IsHost)
        {
            transform.SetPositionAndRotation(Bar.transform.position + new Vector3(0, 0.5f, 0), transform.rotation);
            StartCoroutine(HoldBall());
        }
    }
    private IEnumerator HoldBall()
    {
        m_BallSpeed.Value = 0;
        gameObject.transform.SetParent(m_Bar.transform);
        yield return new WaitForSeconds(m_Penalty.Value);
        ReleaseBall();
        m_BallSpeed.Value = m_MaxSpeed.Value;
    }
    private void ReleaseBall()
    {
        gameObject.transform.SetParent(null);
    }

    public void Stop()
    {
        if (IsHost)
        {
            m_BallSpeed.Value = 0;
        }
    }
}
