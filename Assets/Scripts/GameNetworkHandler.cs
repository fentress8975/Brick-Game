using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkHandler : SingletonNetWork<GameNetworkHandler>
{
    private NetworkManager m_NetworkManager;
    [SerializeField] private string m_SceneName;


#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif

    private void Start()
    {
        m_NetworkManager = NetworkManager.Singleton;
    }

    public void CloseGameConnection()
    {
        m_NetworkManager.Shutdown();
        ReturnToMainMenu();
    }

    private void ReturnToMainMenu()
    {
        var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load {m_SceneName} " +
                  $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }
}
