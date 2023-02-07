using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Action OnDisconnect;

    [SerializeField] private Canvas m_MainCanvas;
    [SerializeField] private Canvas m_CreateLobbyCanvas;
    [SerializeField] private Canvas m_ConnectioToLobbyCanvas;
    [SerializeField] private Canvas m_LobbyCanvas;

    [SerializeField] private Button m_ReturnButton;
    [SerializeField] private Button m_LobbyCanvasButton;
    [SerializeField] private Button m_ConnectionCanvasButton;

    private void Start()
    {
        m_ReturnButton.onClick.AddListener(ReturnToMainMenu);
        m_LobbyCanvasButton.onClick.AddListener(OpenCreateLobbyCanvas);
        m_ConnectionCanvasButton.onClick.AddListener(OpenConnectionToLobbyCanvas);
    }


    private void ReturnToMainMenu()
    {
        if (m_LobbyCanvas.gameObject.activeInHierarchy)
        {
            OnDisconnect?.Invoke();
        }

        m_CreateLobbyCanvas.gameObject.SetActive(false);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(false);
        m_MainCanvas.gameObject.SetActive(true);
        m_LobbyCanvas.gameObject.SetActive(false);

        m_ReturnButton.gameObject.SetActive(false);
    }

    private void OpenCreateLobbyCanvas()
    {
        m_CreateLobbyCanvas.gameObject.SetActive(true);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(false);
        m_MainCanvas.gameObject.SetActive(false);
        m_LobbyCanvas.gameObject.SetActive(false);

        m_ReturnButton.gameObject.SetActive(true);
    }
    private void OpenConnectionToLobbyCanvas()
    {
        m_CreateLobbyCanvas.gameObject.SetActive(false);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(true);
        m_MainCanvas.gameObject.SetActive(false);
        m_LobbyCanvas.gameObject.SetActive(false);

        m_ReturnButton.gameObject.SetActive(true);
    }

    public void OpenLobby()
    {
        m_CreateLobbyCanvas.gameObject.SetActive(false);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(false);
        m_MainCanvas.gameObject.SetActive(false);
        m_LobbyCanvas.gameObject.SetActive(true);


        m_ReturnButton.gameObject.SetActive(true);
    }
}
