using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort Port
    {
        get { return m_Port; }
        set
        {
            if (value > 0 && value <= 65535) { m_Port = value; }
            else { return; }
        }
    }
    private NativeList<NetworkConnection> m_Connections;
    private NetworkEndPoint m_EndPoint;
    private ushort m_Port = 7777;

    private void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_EndPoint = NetworkEndPoint.AnyIpv4;
        m_EndPoint.Port = m_Port;
        if (m_Driver.Bind(m_EndPoint) != 0)
            EditorLogger.Log($"Failed to bind to port {m_Port}");
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        if (m_Driver.IsCreated)
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
    }

    private void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            EditorLogger.Log("Accepted a connection");
        }

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();

                    EditorLogger.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;

                    m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteUInt(number);
                    m_Driver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    EditorLogger.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}
