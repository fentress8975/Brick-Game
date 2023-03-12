using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ClientBarReconciliation : SingletonNetWork<ClientBarReconciliation>
{
    public PlayerClientBar PlayerLocalBar { get => m_PlayerLocalBar; }

    [SerializeField] private PlayerServerBar m_PlayerServerBar;
    [SerializeField] private PlayerClientBar m_PlayerLocalBar;
    [SerializeField] private PlayerClientBar m_PlayerBarPrefab;
    [SerializeField] private Dictionary<ulong, Direction> m_CommandListLocal = new();
    [SerializeField] private Dictionary<ulong, Vector3> m_WaitPosListServer = new();
    [SerializeField] private Dictionary<ulong, Vector3> m_PositionListLocal = new();
    private ulong m_NumberLocal = new();
    private ulong m_FirstUnprocessedCommandNumberLocal = new();
    private ulong m_UnprocessedCommandsCountLocal = new();
    [SerializeField] private TextMeshProUGUI m_UnprocessedCommandsText;

    private Vector3 m_PositionToSync = new();

    public void Init(int id)
    {
        if (id == 1)
        {
            m_PlayerServerBar = GameNetworkHandler.Singletone.Player1Bar;
        }
        else
        {
            m_PlayerServerBar = GameNetworkHandler.Singletone.Player2Bar;
        }
        m_PlayerLocalBar = Instantiate(m_PlayerBarPrefab, m_PlayerServerBar.transform.position, Quaternion.identity);
        m_PlayerLocalBar.transform.position = m_PlayerServerBar.transform.position;
    }

    private void Update()
    {
        CheckWaitList();
    }

    private void ChangeLocalPlayerDirection(Direction directionLocal)
    {
        m_PlayerLocalBar.ChangeDirectionLocal(directionLocal);
    }

    public ulong AddCommandToQueue(Direction directionLocal, Vector3 posLocal)
    {
        ulong commandNumberLocal = m_NumberLocal;
        m_NumberLocal++;
        //EditorLogger.Log($"Добавление команды {commandNumberLocal} направление {directionLocal}, позиция {posLocal}");
        m_CommandListLocal.Add(commandNumberLocal, directionLocal);
        m_PositionListLocal.Add(commandNumberLocal, posLocal);
        m_UnprocessedCommandsCountLocal++;
        m_UnprocessedCommandsText.text = $"Необработанных команд {m_UnprocessedCommandsCountLocal}";
        ChangeLocalPlayerDirection(directionLocal);
        return commandNumberLocal;
    }

    private void CheckWaitList()
    {
        if (m_WaitPosListServer.Count > 0 && m_CommandListLocal.Count > 0)
        {
            if (m_WaitPosListServer.TryGetValue(m_FirstUnprocessedCommandNumberLocal, out Vector3 posServer))
            {
                SyncCompletedCommands(m_FirstUnprocessedCommandNumberLocal, posServer);
            }
            else
            {
                //EditorLogger.Log($"Нету нужной команды {m_FirstUnprocessedCommandNumberLocal}.");
            }
        }
    }

    private void SyncCompletedCommands(ulong numberServer, Vector3 posServer)
    {
        if (iSSyncCommand(numberServer, posServer))
        {
            RemoveCompletedCommand(numberServer);
        }
        else
        {
            //EditorLogger.Log($"Ошибка синхронизации команды {numberServer} :-(");
            Resync(numberServer, posServer);
            return;
        }
    }

    private bool iSSyncCommand(ulong numberServer, Vector3 posServer)
    {
        //EditorLogger.Log($"Синхронизация команды {numberServer} сервера");
        if (m_PositionListLocal[m_FirstUnprocessedCommandNumberLocal] == posServer)
        {
            return true;
        }
        else
        {
            //EditorLogger.Log($"{m_PositionListLocal[numberServer]} не совпадает с {m_WaitPosListServer[numberServer]}");
            return false;
        }
    }

    private void RemoveCompletedCommand(ulong numberServer)
    {
        //EditorLogger.Log($"Удаление команды {numberServer}");
        m_CommandListLocal.Remove(numberServer);
        m_PositionListLocal.Remove(numberServer);
        m_WaitPosListServer.Remove(numberServer);
        m_UnprocessedCommandsCountLocal--;
        m_FirstUnprocessedCommandNumberLocal = numberServer + 1;
        m_UnprocessedCommandsText.text = $"Необработанных команд {m_UnprocessedCommandsCountLocal}";
    }

    private void Resync(ulong numberServer, Vector3 posServer)
    {
        Vector3 posDiff = posServer - m_PositionListLocal[numberServer];
        RemoveCompletedCommand(numberServer);

        var m_CopyListKeys = m_CommandListLocal.Keys.ToList();
        m_CopyListKeys.Sort();
        foreach (var key in m_CopyListKeys)
        {
            m_PositionListLocal[key] = m_PositionListLocal[key] + posDiff;
           // EditorLogger.Log($"Команда {key} имеет новую позицию {m_PositionListLocal[key]}");
        }

        Vector3 _newLocalPos = m_PlayerLocalBar.transform.position + posDiff;
        SyncLocalBarPosition(posDiff);
    }

    private void SyncLocalBarPosition(Vector3 newLocalPos)
    {
        StopAllCoroutines();
        m_PositionToSync = newLocalPos;
        StartCoroutine(WaitToSyncPlayerPosition());
    }

    private IEnumerator WaitToSyncPlayerPosition()
    {
        while (m_PlayerLocalBar.PlayerDirection != Direction.None && m_UnprocessedCommandsCountLocal == 0)
        {
            yield return null;
        }
        //EditorLogger.Log($"Синхронизирую позицию игрока");
        m_PlayerLocalBar.transform.position += m_PositionToSync;
        m_PositionToSync = Vector3.zero;
    }

    private void AddCommandToWaitList(ulong numberServer, Vector3 posServer)
    {
        m_WaitPosListServer.Add(numberServer, posServer);
        //EditorLogger.Log($"Ожидают {m_WaitPosListServer.Count} команд.");
    }

    [ClientRpc]
    public void ReturnCompletedCommandClientRpc(ulong number, Vector3 pos, ClientRpcParams clientRpcParams = default)
    {
        //EditorLogger.Log($"Получение команды {number}, позиция {pos}");
        AddCommandToWaitList(number, pos);
    }

    [ClientRpc]
    public void InitClientRpc(int number, ClientRpcParams clientRpcParams = default)
    {
        Init(number);
    }
}