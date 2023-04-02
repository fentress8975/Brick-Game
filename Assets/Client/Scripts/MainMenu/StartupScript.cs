using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupScript : MonoBehaviour
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_MainMenuScene = SceneAsset.name;
        }
    }
#endif

    [SerializeField] private string m_MainMenuScene;


    private void Start()
    {
        SceneManager.LoadScene(m_MainMenuScene, LoadSceneMode.Single);
    }
}
