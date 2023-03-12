using UnityEngine;

public class ClientSettings : MonoBehaviour
{
    public int targetFrameRate = 60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
    private void Update()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
