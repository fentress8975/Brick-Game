using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerServerBall : NetworkBehaviour
{
    public float BallSpeed { get => m_BallSpeed.Value; }
    public float MaxSpeed { get => m_MaxSpeed.Value; }
    public float Penalty { get => m_Penalty.Value; }

    [SerializeField] private NetworkVariable<float> m_BallSpeed = new(0);
    [SerializeField] private NetworkVariable<float> m_MaxSpeed = new(5);
    [SerializeField] private NetworkVariable<float> m_OldMaxSpeed = new(5);
    [SerializeField] private NetworkVariable<float> m_Penalty = new(1.5f);
    [SerializeField] private NetworkVariable<ulong> m_PlayerId = new();
    [SerializeField] private PlayerServerBar m_PlayerBar;

    private Rigidbody m_Rigidbody;


    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(PlayerServerBar bar, ulong id)
    {
        m_PlayerBar = bar;
        m_PlayerId.Value = id;
    }

    private void FixedUpdate()
    {
        if (IsHost)
        {
            CheckSpeed();
            Move();
        }
    }

    private void CheckSpeed()
    {
        if (m_BallSpeed.Value < m_MaxSpeed.Value || m_BallSpeed.Value > m_MaxSpeed.Value)
        {
            m_BallSpeed.Value = m_MaxSpeed.Value;
        }
    }

    private void Move()
    {
        if (m_BallSpeed.Value <= 0)
        {
            return;
        }
        m_Rigidbody.MovePosition(transform.position + m_BallSpeed.Value * Time.fixedDeltaTime * transform.TransformDirection(Vector3.forward));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsHost) { return; }
        Collider _collider = collision.collider;
        Vector3 _collisionPoint = _collider.ClosestPointOnBounds(transform.position) + transform.TransformDirection(Vector3.back * 5);
        Ray _directionRay = new(_collisionPoint, transform.TransformDirection(Vector3.forward));
        if (_collider.Raycast(_directionRay, out RaycastHit hitInfo, Mathf.Infinity))
        {
            m_Rigidbody.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.TransformDirection(Vector3.forward), hitInfo.normal), Vector3.back);
        }
        SyncPlayerBallData(gameObject.transform.position, gameObject.transform.rotation);
    }



    public void ResetBall()
    {
        if (IsHost)
        {
            StartCoroutine(HoldBall());
        }
    }
    private IEnumerator HoldBall()
    {
        float timeLeft = m_Penalty.Value;
        while (timeLeft > 0)
        {
            m_BallSpeed.Value = 0;
            transform.position = m_PlayerBar.transform.position + new Vector3(0, 0.5f, 0);
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        ReleaseBall();
        //SyncPlayerBallData(gameObject.transform.position, gameObject.transform.rotation);
    }


    private void ReleaseBall()
    {
        m_MaxSpeed.Value = 5;
    }

    public void Stop()
    {
        if (IsHost)
        {

            StopAllCoroutines();
            m_MaxSpeed.Value = 0;
        }
    }

    private void SyncPlayerBallData(Vector3 pos, Quaternion angle)
    {
        if (m_PlayerId.Value == GameNetworkHandler.Singletone.Player1ID)
        {
            ClientBallReconciliation.Singletone.ReturnCompletedCommandClientRpc(
                pos, angle, GameNetworkHandler.Singletone.Player1RpcParams);
        }
        else if (m_PlayerId.Value == GameNetworkHandler.Singletone.Player2ID)
        {
            ClientBallReconciliation.Singletone.ReturnCompletedCommandClientRpc(
               pos, angle, GameNetworkHandler.Singletone.Player2RpcParams);
        }
        else
        {
            EditorLogger.Log("Vse ochen ploxo");
        }
    }
}
