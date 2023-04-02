using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Action OnDisconnect;
    public Action<string> OnAdressChange;
    public Action<string> OnPortChange;
    public Action OnHostStart;
    public Action OnConnectionStart;
    [Header("Canvases")]
    [SerializeField] private Canvas m_MainCanvas;
    [SerializeField] private Canvas m_StartHostCanvas;
    [SerializeField] private Canvas m_ConnectioToLobbyCanvas;
    [SerializeField] private Canvas m_LobbyCanvas;
    [Header("Buttons")]
    [SerializeField] private Button m_ReturnButton;
    [SerializeField] private Button m_HostCanvasButton;
    [SerializeField] private Button m_ConnectCanvasButton;
    [SerializeField] private Button m_StartHostButton;
    [SerializeField] private Button m_StartConnectionButton;
    [SerializeField] private Button m_CloseApplicationButton;
    [Header("Input Fields")]

    [SerializeField] private TMP_InputField m_AdressInput;
    [SerializeField] private TMP_InputField m_PortHostInput;
    [SerializeField] private TMP_InputField m_PortConnectInput;

    [SerializeField] private TMP_InputField m_PlayerNickNameInputField;

    public TMP_InputField PortConnectInput { get => m_PortConnectInput; set => m_PortConnectInput = value; }
    public TMP_InputField AdressInput { get => m_AdressInput; set => m_AdressInput = value; }
    public Button StartConnectionButton { get => m_StartConnectionButton; set => m_StartConnectionButton = value; }
    public Button StartHostButton { get => m_StartHostButton; set => m_StartHostButton = value; }
    public string PlayerNickName { get => m_PlayerNickNameInputField.text; set => m_PlayerNickNameInputField.text = value; }

    private void Start()
    {
        m_ReturnButton.onClick.AddListener(ReturnToMainMenu);
        m_HostCanvasButton.onClick.AddListener(OpenCreateLobbyCanvas);
        m_ConnectCanvasButton.onClick.AddListener(OpenConnectionToLobbyCanvas);

        m_StartConnectionButton.onClick.AddListener(() => { OnConnectionStart?.Invoke(); });
        m_StartHostButton.onClick.AddListener(() => { OnHostStart?.Invoke(); });
        m_CloseApplicationButton.onClick.AddListener(QuitGame);

        m_AdressInput.onValueChanged.AddListener(ChangeAdress);
        m_PortHostInput.onValueChanged.AddListener(ChangePort);
        m_PortConnectInput.onValueChanged.AddListener(ChangePort);

        m_PlayerNickNameInputField.onValueChanged.AddListener((string name) =>
        {
            PlayerPrefs.SetString("Player1Nickname", m_PlayerNickNameInputField.text);
            PlayerPrefs.Save();
        });
        EditorLogger.Log(PlayerPrefs.HasKey("Player1Nickname").ToString());
        m_PlayerNickNameInputField.text = PlayerPrefs.HasKey("Player1Nickname") ? PlayerPrefs.GetString("Player1Nickname") : "Player";

    }


    public void ReturnToMainMenu()
    {
        if (m_LobbyCanvas.gameObject.activeInHierarchy)
        {
            OnDisconnect?.Invoke();
        }

        m_StartHostCanvas.gameObject.SetActive(false);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(false);
        m_MainCanvas.gameObject.SetActive(true);
        m_LobbyCanvas.gameObject.SetActive(false);

        m_ReturnButton.gameObject.SetActive(false);
    }

    private void OpenCreateLobbyCanvas()
    {
        m_StartHostCanvas.gameObject.SetActive(true);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(false);
        m_MainCanvas.gameObject.SetActive(false);
        m_LobbyCanvas.gameObject.SetActive(false);

        m_ReturnButton.gameObject.SetActive(true);
    }
    private void OpenConnectionToLobbyCanvas()
    {
        m_StartHostCanvas.gameObject.SetActive(false);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(true);
        m_MainCanvas.gameObject.SetActive(false);
        m_LobbyCanvas.gameObject.SetActive(false);

        m_ReturnButton.gameObject.SetActive(true);
    }

    public void OpenLobby()
    {
        m_StartHostCanvas.gameObject.SetActive(false);
        m_ConnectioToLobbyCanvas.gameObject.SetActive(false);
        m_MainCanvas.gameObject.SetActive(false);
        m_LobbyCanvas.gameObject.SetActive(true);


        m_ReturnButton.gameObject.SetActive(true);
    }

    private void ChangeAdress(string adress)
    {
        OnAdressChange?.Invoke(adress);
    }

    private void ChangePort(string port)
    {
        m_PortConnectInput.text = port;
        m_PortHostInput.text = port;
        OnPortChange?.Invoke(port);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
