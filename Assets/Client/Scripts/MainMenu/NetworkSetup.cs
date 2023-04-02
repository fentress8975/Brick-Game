using System.Collections;
#if UNITY_STANDALONE
using System.Net;
using System.Net.Sockets;
#endif
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkSetup : SingletonMono<NetworkSetup>
{
    private const string DEFAULT_ADDRESS = "127.0.0.1";
    private const string DEFAULT_PORT = "7777";

    private NetworkManager m_NetworkManager;
    private UnityTransport m_UTransport;

    [SerializeField] private MainMenuUI m_MainMenuUI;

    //DebugArea
    [SerializeField] private TextMeshProUGUI m_IPAdress;
    [SerializeField] private TextMeshProUGUI m_PortAdress;

    private void Start()
    {
        m_NetworkManager = NetworkManager.Singleton;
        m_UTransport = m_NetworkManager.GetComponent<UnityTransport>();
        m_MainMenuUI.OnDisconnect += Disconnect;
        m_MainMenuUI.OnHostStart += StartHost;
        m_MainMenuUI.OnConnectionStart += StartClient;
        m_MainMenuUI.OnPortChange += ChangePort;
        m_MainMenuUI.OnAdressChange += ChangeAdress;
    }

    private void OnDestroy()
    {
        m_NetworkManager.OnClientConnectedCallback -= Connected;
    }

    private void StartHost()
    {
#if UNITY_EDITOR
        ChangeAdress(DEFAULT_ADDRESS);
#else
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            ChangeAdress(localIP);
#endif
        m_NetworkManager.GetComponent<UnityTransport>().ConnectionData.ServerListenAddress = "0.0.0.0";

        if (m_NetworkManager.StartHost())
        {
            ChangeUIPortIP();
            m_MainMenuUI.OpenLobby();
        }
        else
        {
            m_IPAdress.text = "IP: Error";
            m_PortAdress.text = "Port: Error";
        }

    }

    private void StartClient()
    {
        if (m_NetworkManager.StartClient())
        {
            m_NetworkManager.OnClientConnectedCallback += Connected;
            StartCoroutine(TryToConnect());
        }
        else
        {
            EditorLogger.Log("Failed to start Client");
        }
    }

    private void ChangeAdress(string adress)
    {
        if (IPAddress.TryParse(adress, out IPAddress iPAdress))
        {
            m_UTransport.SetConnectionData(iPAdress.ToString(), m_UTransport.ConnectionData.Port);
        }
        else
        {
            m_UTransport.SetConnectionData(DEFAULT_ADDRESS, m_UTransport.ConnectionData.Port);

        }
    }

    private void ChangePort(string port)
    {
        m_UTransport.SetConnectionData(m_UTransport.ConnectionData.Address, ushort.Parse(port));
    }

    private void Connected(ulong obj)
    {
        StopAllCoroutines();
        ChangeUIPortIP();
        m_MainMenuUI.OpenLobby();
    }

    private IEnumerator TryToConnect()
    {
        int attempts = 5;
        while (attempts > 0)
        {
            attempts--;
            EditorLogger.Log("Try to connect");
            yield return new WaitForSeconds(2f);
        }
        FailedToConnect();
    }

    private void ChangeUIPortIP()
    {
        EditorLogger.Log("Connect Success");
        m_IPAdress.text = $"IP: {m_NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address}";
        m_PortAdress.text = $"Port: {m_NetworkManager.GetComponent<UnityTransport>().ConnectionData.Port}";
    }

    private void FailedToConnect()
    {
        EditorLogger.Log("FaildeToConnect");
        m_IPAdress.text = "IP: Error";
        m_PortAdress.text = "Port: Error";
        NetworkManager.Singleton.Shutdown();
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        m_IPAdress.text = "IP: Disconnected";
        m_PortAdress.text = "Port: Disconnected";
    }
}
