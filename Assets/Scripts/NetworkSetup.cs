using System.Collections;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSetup : SingletonMono<NetworkSetup>
{
    private const string DEFAULT_ADDRESS = "127.0.0.1";
    private const string DEFAULT_PORT = "7777";

    [SerializeField] private MainMenuUI m_MainMenuUI;

    [SerializeField] private Button m_StartHostButton;
    [SerializeField] private Button m_StartClientButton;
    [SerializeField] private TMP_InputField m_HostPortInputField;
    [SerializeField] private TMP_InputField m_ClientIPAdressInputField;
    [SerializeField] private TMP_InputField m_ClientPortInputField;
    [SerializeField] private TMP_InputField m_PlayerNickNameInputField;

    //DebugArea
    [SerializeField] private TextMeshProUGUI m_IPAdress;
    [SerializeField] private TextMeshProUGUI m_PortAdress;

    private void Start()
    {
        m_MainMenuUI.OnDisconnect += Disconnect;

        m_StartHostButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            ChangeServerAdress(DEFAULT_ADDRESS);
#else
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            ChangeServerAdress(localIP);
#endif

            if (NetworkManager.Singleton.StartHost())
            {
                m_IPAdress.text = $"IP: {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address}";
                m_PortAdress.text = $"Port: {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port}";
                m_MainMenuUI.OpenLobby();
            }
            else
            {
                m_IPAdress.text = "IP: Error";
                m_PortAdress.text = "Port: Error";
            }

        });
        m_StartClientButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())
            {
                StartCoroutine(TryToConnect());
            }
            else
            {
                EditorLogger.Log("Failed to start Client");
            }

        });

        m_HostPortInputField.onSubmit.AddListener((string adress) =>
        {
            if (ushort.TryParse(adress, out ushort portUshort))
            {
                if (portUshort > 0 && portUshort <= 65535)
                {
                    m_HostPortInputField.text = portUshort.ToString();
                }
                else
                {
                    m_HostPortInputField.text = DEFAULT_PORT;
                }
            }
            ChangeServerPort(m_HostPortInputField.text);
        });

        m_ClientIPAdressInputField.onSubmit.AddListener((string adress) =>
        {
            if (IPAddress.TryParse(adress, out IPAddress iPAdress))
            {

                m_ClientIPAdressInputField.text = iPAdress.ToString();
            }
            else
            {
                m_ClientIPAdressInputField.text = DEFAULT_ADDRESS;
            }
            ChangeServerAdress(m_ClientIPAdressInputField.text);
        });
        m_ClientPortInputField.onSubmit.AddListener((string port) =>
        {
            if (ushort.TryParse(port, out ushort portUshort))
            {
                if (portUshort > 0 && portUshort <= 65535)
                {
                    m_ClientPortInputField.text = portUshort.ToString();
                }
                else
                {
                    m_ClientPortInputField.text = DEFAULT_PORT;
                }
            }
            ChangeServerPort(m_ClientPortInputField.text);
        });
    }
    private IEnumerator TryToConnect()
    {
        int attempts = 5;
        while (attempts > 0)
        {
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                ClientConnected();
                yield break;
            }
            else
            {
                attempts--;
            }
            EditorLogger.Log("Try to connect");
            yield return new WaitForSeconds(2f);
        }
        FailedToConnect();
    }

    private void ClientConnected()
    {
        EditorLogger.Log("Connect Success");
        m_IPAdress.text = $"IP: {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address}";
        m_PortAdress.text = $"Port: {NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port}";
        m_MainMenuUI.OpenLobby();
    }

    private void FailedToConnect()
    {
        EditorLogger.Log("FaildeToConnect");
        m_IPAdress.text = "IP: Error";
        m_PortAdress.text = "Port: Error";
        NetworkManager.Singleton.Shutdown();
    }

    private void ChangeServerAdress(string adress)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(adress, ushort.Parse(m_ClientPortInputField.text));
    }

    private void ChangeServerPort(string port)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(m_ClientIPAdressInputField.text, ushort.Parse(port));
        m_ClientPortInputField.text = port;
        m_HostPortInputField.text = port;
    }

    private void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        m_IPAdress.text = "IP: Disconnected";
        m_PortAdress.text = "Port: Disconnected";
    }
}
