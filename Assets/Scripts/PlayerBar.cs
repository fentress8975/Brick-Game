using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerBar : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<Direction> m_DirectionServer = new(Direction.None);
    private NetworkVariable<float> m_MaxSpeed = new(5f);
    private NetworkVariable<float> m_CurrentSpeed = new(0);

    private void Update()
    {
        if (m_DirectionServer.Value == Direction.None) { return; }
        switch (m_DirectionServer.Value)
        {
            case Direction.Left:
                transform.position += m_MaxSpeed.Value * Time.deltaTime * transform.TransformDirection(Vector3.left);
                break;
            case Direction.Right:
                transform.position += m_MaxSpeed.Value * Time.deltaTime * transform.TransformDirection(Vector3.right);
                break;
            default:
                break;
        }
    }

    public void ChangeDirection(Direction direction)
    {
        EditorLogger.Log(gameObject.name);
        m_DirectionServer.Value = direction;
    }

    internal void Stop()
    {
        throw new NotImplementedException();
    }

    internal void ResetBar()
    {
        throw new NotImplementedException();
    }
}
