using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool m_Done;
    public ushort Port
    {
        get { return m_Port; }
        set
        {
            if (value > 0 && value <= 65535) { m_Port = value; }
            else { return; }
        }
    }

    private NetworkEndPoint m_EndPoint;
    private ushort m_Port = 7777;

    private void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);

        m_EndPoint = NetworkEndPoint.LoopbackIpv4;
        m_EndPoint.Port = m_Port;
        m_Connection = m_Driver.Connect(m_EndPoint);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    private void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!m_Done)
                EditorLogger.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                EditorLogger.Log("We are now connected to the server");

                uint value = 1;
                m_Driver.BeginSend(m_Connection, out var writer);
                writer.WriteUInt(value);
                m_Driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                EditorLogger.Log("Got the value = " + value + " back from the server");
                m_Done = true;
                m_Connection.Disconnect(m_Driver);
                m_Connection = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                EditorLogger.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
            }
        }
    }
}
