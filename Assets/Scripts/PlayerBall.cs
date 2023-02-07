using Unity.Netcode;
using UnityEngine;

public class PlayerBall : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<float> m_BallSpeed = new(0);
    [SerializeField] private NetworkVariable<float> m_MaxSpeed = new(5);
    [SerializeField] private NetworkVariable<Vector2> m_StartPosition = new();

    private Rigidbody m_Rigidbody;

    [SerializeField] private NetworkVariable<ulong> m_PlayerId = new();


    [SerializeField] private PlayerBar m_Bar;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(PlayerBar playerBar, ulong id)
    {

    }

    private void Update()
    {
        if (m_BallSpeed.Value <= 0)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        m_Rigidbody.MovePosition(transform.position + m_BallSpeed.Value * Time.deltaTime * transform.TransformDirection(Vector3.forward));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider _collider = collision.collider;
        Vector3 _collisionPoint = _collider.ClosestPointOnBounds(transform.position) + transform.TransformDirection(Vector3.back * 5);
        Ray _directionRay = new(_collisionPoint,transform.TransformDirection(Vector3.forward));

        if (_collider.Raycast(_directionRay,out RaycastHit hitInfo, Mathf.Infinity))
        {
            EditorLogger.Log("RayCast");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.red, 10);
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 10, Color.green, 10);
            Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.TransformDirection(Vector3.forward), hitInfo.normal) * 10, Color.blue, 10);
            Debug.Log(Vector3.Reflect(transform.TransformDirection(Vector3.forward), hitInfo.normal));
            m_Rigidbody.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.TransformDirection(Vector3.forward), hitInfo.normal),Vector3.back);
        }

    }

    [ClientRpc]
    public void RestartBallClientRpc()
    {

    }
    [ClientRpc]
    public void StopBallClientRpc()
    {
        m_BallSpeed.Value = 0;
    }
    [ClientRpc]
    public void StartBallClientRpc()
    {
        m_BallSpeed.Value = m_MaxSpeed.Value;
    }
}
