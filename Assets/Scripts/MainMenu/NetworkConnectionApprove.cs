using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionApprove : MonoBehaviour
{
    private const int MAX_PLAYERS = 2;

    private NetworkManager m_NetworkManager;
    private const int k_MaxConnectPayload = 1024;



    private void Start()
    {
        m_NetworkManager = NetworkManager.Singleton.GetComponent<NetworkManager>();
        if (m_NetworkManager != null)
        {
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            m_NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        }
        m_NetworkManager.NetworkConfig.ConnectionApproval = true;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var connectionData = request.Payload;
        var clientId = request.ClientNetworkId;
        if (connectionData.Length > k_MaxConnectPayload)
        {
            response.Approved = false;
            return;
        }

        if (CheckForFreePlayerSlot())
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = null;
        }
        else
        {
            response.Approved = false;
        }
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        EditorLogger.Log($"Disconnected {obj}");
    }

    private bool CheckForFreePlayerSlot()
    {
        if (m_NetworkManager.ConnectedClientsIds.Count >= MAX_PLAYERS)
        {
            EditorLogger.Log($"mest netu");
            return false;
        }
        else
        {
            EditorLogger.Log("mesta est");
            return true;
        }
    }

    private void OnDestroy()
    {
        m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        m_NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
    }
}
