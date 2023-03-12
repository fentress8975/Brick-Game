using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class ClientBallReconciliation : SingletonNetWork<ClientBallReconciliation>
{
    public PlayerClientBall PlayerLocalBar { get => m_PlayerLocalBall; }
    public PlayerServerBall PlayerServerBall { get => m_PlayerServerBall;}

    [SerializeField] private PlayerServerBall m_PlayerServerBall;
    [SerializeField] private PlayerClientBall m_PlayerLocalBall;
    [SerializeField] private PlayerClientBall m_PlayerBarPrefab;
    [SerializeField] private Dictionary<ulong, BallPositionAngle> m_BallPositionsAnglesLocalDictionary = new();
    [SerializeField] private Dictionary<ulong, BallPositionAngle> m_BallPositionsAnglesServerDictionary = new();


    private ulong m_NumberLocal = new();
    private ulong m_NumberServer = new();
    private ulong m_FirstUnprocessedCommandNumberLocal = new();
    private ulong m_UnprocessedCommandsCountLocal = new();
    [SerializeField] private TextMeshProUGUI m_UnprocessedCommandsText;

    public void Init(int id)
    {
        if (id == 1)
        {
            m_PlayerServerBall = GameNetworkHandler.Singletone.Player1Ball;
        }
        else
        {
            m_PlayerServerBall = GameNetworkHandler.Singletone.Player2Ball;
        }
        m_PlayerLocalBall = Instantiate(m_PlayerBarPrefab, m_PlayerServerBall.transform.position, m_PlayerServerBall.transform.rotation);
        m_PlayerLocalBall.transform.rotation = m_PlayerServerBall.transform.rotation;
        m_PlayerLocalBall.Setup(m_PlayerServerBall.BallSpeed, m_PlayerServerBall.MaxSpeed, m_PlayerServerBall.Penalty, ClientBarReconciliation.Singletone.PlayerLocalBar);
        m_PlayerLocalBall.onCollision += AddAngleToQueue;
        m_PlayerLocalBall.ResetBall();
    }

    private void Update()
    {
        CheckWaitList();
    }

    public void AddAngleToQueue(Vector3 posLocal, Quaternion angleLocal)
    {
        ulong commandNumberLocal = m_NumberLocal;
        EditorLogger.Log($"Добавление команды {commandNumberLocal}, позиция {posLocal}, angle {angleLocal}");
        m_BallPositionsAnglesLocalDictionary.Add(commandNumberLocal, new BallPositionAngle(posLocal, angleLocal));
        m_NumberLocal++;
        m_UnprocessedCommandsCountLocal++;
    }

    private void CheckWaitList()
    {
        if (m_BallPositionsAnglesServerDictionary.Count > 0 && m_BallPositionsAnglesLocalDictionary.Count > 0)
        {
            EditorLogger.Log($"Поиск команд.");
            if (m_BallPositionsAnglesServerDictionary.TryGetValue(m_FirstUnprocessedCommandNumberLocal, out BallPositionAngle ballServerData))
            {
                SyncCompletedCommands(m_FirstUnprocessedCommandNumberLocal, ballServerData);
            }
            else
            {
                EditorLogger.Log($"Нету нужной команды {m_FirstUnprocessedCommandNumberLocal}.");
            }
        }
    }

    private void SyncCompletedCommands(ulong numberServer, BallPositionAngle ballServerData)
    {
        if (isSyncCommand(numberServer, ballServerData))
        {
            RemoveCompletedCommand(numberServer);
        }
        else
        {
            Resync(numberServer,ballServerData);
            return;
        }
    }

    private bool isSyncCommand(ulong numberServer, BallPositionAngle ballServerData)
    {
        if (m_BallPositionsAnglesLocalDictionary[m_FirstUnprocessedCommandNumberLocal] == ballServerData)
        {
            return true;
        }
        else
        {
            EditorLogger.Log($" Command {numberServer} {m_BallPositionsAnglesLocalDictionary[numberServer].Angle} не совпадает с {m_BallPositionsAnglesServerDictionary[numberServer].Angle}");
            return false;
        }
    }

    private void RemoveCompletedCommand(ulong numberServer)
    {
        EditorLogger.Log($"Удаление команды {numberServer}");
        m_BallPositionsAnglesLocalDictionary.Remove(numberServer);
        m_BallPositionsAnglesServerDictionary.Remove(numberServer);
        m_UnprocessedCommandsCountLocal--;
        m_FirstUnprocessedCommandNumberLocal = numberServer + 1;
        m_UnprocessedCommandsText.text = $"Необработанных команд {m_UnprocessedCommandsCountLocal}";
    }

    private void Resync(ulong numberServer, BallPositionAngle ballServerData)
    {
        m_PlayerLocalBall.gameObject.transform.position = ballServerData.Position;
        m_PlayerLocalBall.gameObject.transform.rotation = ballServerData.Angle;
        m_NumberLocal = numberServer;
        m_BallPositionsAnglesLocalDictionary.Clear();
        //m_PlayerLocalBall.UpdateSpeed(m_PlayerServerBall.BallSpeed, m_PlayerServerBall.MaxSpeed, m_PlayerServerBall.Penalty);
        RemoveCompletedCommand(numberServer);
    }

    private void AddCommandToWaitList(BallPositionAngle ballDataServer)
    {
        ulong _number = m_NumberServer;
        EditorLogger.Log($"Получение команды {_number}, позиция {ballDataServer.Position}, angle {ballDataServer.Angle}");
        m_BallPositionsAnglesServerDictionary.Add(_number, ballDataServer);
        EditorLogger.Log($"Ожидают {m_BallPositionsAnglesServerDictionary.Count} команд мяча.");
        m_NumberServer++;
    }

    [ClientRpc]
    public void ReturnCompletedCommandClientRpc(Vector3 ballPos, Quaternion ballAngle, ClientRpcParams clientRpcParams = default)
    {
        BallPositionAngle ballData = new(ballPos, ballAngle);
        AddCommandToWaitList(ballData);
    }

    [ClientRpc]
    public void InitClientRpc(int number, ClientRpcParams clientRpcParams = default)
    {
        Init(number);
    }
}

public struct BallPositionAngle
{
    public Vector3 Position { get => m_Position; set => m_Position = value; }
    public Quaternion Angle { get => m_Angle; set => m_Angle = value; }

    private Vector3 m_Position;
    private Quaternion m_Angle;

    public BallPositionAngle(Vector3 position, Quaternion angle)
    {
        m_Position = position;
        m_Angle = angle;
    }

    public static bool operator ==(BallPositionAngle one, BallPositionAngle two)
    {
        if (one.Position == two.Position && one.Angle == two.Angle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator !=(BallPositionAngle one, BallPositionAngle two)
    {
        if (one.Position != two.Position || one.Angle != two.Angle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
