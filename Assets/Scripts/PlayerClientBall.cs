using System;
using System.Collections;
using UnityEngine;


public class PlayerClientBall : MonoBehaviour
{
    public Action<Vector3, Quaternion> onCollision;

    public float BallSpeed { get => m_BallSpeed; }
    public float MaxSpeed { get => m_MaxSpeed; }
    public float Penalty { get => m_Penalty; }
    [Header("Local Variables")]
    [SerializeField] private float m_BallSpeed = 0;
    [SerializeField] private float m_MaxSpeed = 5;
    [SerializeField] private float m_Penalty = 1.5f;
    [SerializeField] private PlayerClientBar m_PlayerBar;
    private ClientBallReconciliation m_ClientBallReconciliation;
    private Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(float ballSpeed, float maxSpeed, float penalty, PlayerClientBar bar)
    {
        m_BallSpeed = ballSpeed;
        m_MaxSpeed = maxSpeed;
        m_Penalty = penalty;
        m_PlayerBar = bar;
        m_ClientBallReconciliation = ClientBallReconciliation.Singletone;
    }

    private void FixedUpdate()
    {
        CheckSpeed();
        Move();
    }

    private void Move()
    {
        if (m_BallSpeed <= 0)
        {
            return;
        }
        m_Rigidbody.MovePosition(transform.position + m_BallSpeed * Time.fixedDeltaTime * transform.TransformDirection(Vector3.forward));
    }

    private void CheckSpeed()
    {
        if (m_BallSpeed != m_ClientBallReconciliation.PlayerServerBall.BallSpeed) { m_BallSpeed = m_ClientBallReconciliation.PlayerServerBall.BallSpeed; }
        if (m_MaxSpeed != m_ClientBallReconciliation.PlayerServerBall.MaxSpeed) { m_MaxSpeed = m_ClientBallReconciliation.PlayerServerBall.MaxSpeed; }
        if (m_BallSpeed < m_MaxSpeed || m_BallSpeed > m_MaxSpeed)
        {
            m_BallSpeed = m_MaxSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EditorLogger.LogWarning(collision.gameObject.name.ToString());
        //do not collide with serverbar
        //if (collision.gameObject.TryGetComponent<PlayerServerBar>(out _))
        //{
        //    return;
        //}

        Collider _collider = collision.collider;
        Vector3 _collisionPoint = _collider.ClosestPointOnBounds(transform.position) + transform.TransformDirection(Vector3.back * 5);
        Ray _directionRay = new(_collisionPoint, transform.TransformDirection(Vector3.forward));
        if (_collider.Raycast(_directionRay, out RaycastHit hitInfo, Mathf.Infinity))
        {
            m_Rigidbody.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.TransformDirection(Vector3.forward), hitInfo.normal), Vector3.back);
        }
        onCollision?.Invoke(gameObject.transform.position, gameObject.transform.rotation);
    }
  
    public void UpdateSpeed(float ballSpeed, float maxSpeed, float penalty)
    {
        m_BallSpeed = ballSpeed;
        m_MaxSpeed = maxSpeed;
        m_Penalty = penalty;
    }

    public void ResetBall()
    {
        onCollision?.Invoke(gameObject.transform.position, gameObject.transform.rotation);
        StartCoroutine(HoldBall());
    }
    private IEnumerator HoldBall()
    {
        float timeLeft = m_Penalty;
        while (timeLeft > 0)
        {
            m_BallSpeed = 0;
            transform.position = m_PlayerBar.transform.position + new Vector3(0, 0.5f, 0);
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        ReleaseBall();
        onCollision?.Invoke(gameObject.transform.position, gameObject.transform.rotation);
    }
    private void ReleaseBall()
    {
        //do smth cool efx
    }

    public void Stop()
    {
        StopAllCoroutines();
        m_BallSpeed = 0;
    }


}
