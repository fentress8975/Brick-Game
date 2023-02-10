using Unity.Netcode;
using UnityEngine;

public class ServerBarControls : MonoBehaviour
{
    [SerializeField] private PlayerBar m_Player1;
    [SerializeField] private PlayerBar m_Player2;
    [SerializeField] private ulong m_Player1Id = 999;
    [SerializeField] private ulong m_Player2Id = 999;


    public void SetupPlayers(ulong player1, ulong player2)
    {
        m_Player1Id = player1;
        m_Player2Id = player2;
    }

    public void ChangePlayerBarDirectionServerRpc(Direction direction, ulong id)
    {
        ulong clientId = id;
        if(clientId == m_Player1Id)
        {
            m_Player1.ChangeDirection(direction);
        }
        else if(clientId == m_Player2Id)
        {
            m_Player2.ChangeDirection(direction);
        }
        else
        {
            EditorLogger.Log("Vse ochen ploxo");
        }
    }

}
