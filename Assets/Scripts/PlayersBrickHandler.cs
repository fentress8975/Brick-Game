using System;
using Unity.Netcode;
using UnityEngine;

public class PlayersBrickHandler : NetworkBehaviour
{
    public Action OnPlayer1Victory;
    public Action OnPlayer2Victory;

    [SerializeField] private BrickGenerator m_BrickGenerator;
    private PlayersBricks m_PlayersBrickList;

#if UNITY_EDITOR
    [SerializeField] private GameManager m_GameManager;
#endif

    public void GeneratePlayerAreasSymmetrical()
    {
        ClearBricks();
        if (IsHost)
        {
            m_PlayersBrickList = m_BrickGenerator.GenerateSymmetrically(GenerateRandomAreaBricks());
            foreach (var item in m_PlayersBrickList.P1Bricks)
            {
                item.OnDestruction += CheckVictoryConditionP1;
            }
            foreach (var item in m_PlayersBrickList.P2Bricks)
            {
                item.OnDestruction += CheckVictoryConditionP2;
            }
        }
    }

    private void ClearBricks()
    {
        if (m_PlayersBrickList != null)
        {
            if (m_PlayersBrickList.P1Bricks != null)
            {
                foreach (var item in m_PlayersBrickList.P1Bricks)
                {
                    item.OnNetworkDespawn();
                    Destroy(item.gameObject);
                }
            }
            if (m_PlayersBrickList.P2Bricks != null)
            {
                foreach (var item in m_PlayersBrickList.P2Bricks)
                {
                    item.OnNetworkDespawn();
                    Destroy(item.gameObject);
                }
            }
        }
    }

    private void CheckVictoryConditionP1(Brick obj)
    {
        m_PlayersBrickList.P1Bricks.Remove(obj);
        if (m_PlayersBrickList.P1Bricks.Count == 0)
        {
            EditorLogger.Log($"{this.name} Player1 Victory!");
            OnPlayer1Victory?.Invoke();
        }
    }

    private void CheckVictoryConditionP2(Brick obj)
    {
        m_PlayersBrickList.P2Bricks.Remove(obj);
        if (m_PlayersBrickList.P2Bricks.Count == 0)
        {
            EditorLogger.Log($"{this.name} Player2 Victory!");
            OnPlayer2Victory?.Invoke();
        }
    }

    private bool[,] GenerateRandomAreaBricks()
    {
        bool[,] bools = new bool[6, 8];
        for (int i = 0; i < bools.GetLength(0); i++)
        {
            for (int j = 0; j < bools.GetLength(1); j++)
            {
                bools[i, j] = UnityEngine.Random.value > 0.5f;
            }
        }
        return bools;
    }
#if UNITY_EDITOR
    [ContextMenu("P1 victory")]
    private void KillBricks()
    {
        foreach (var item in m_PlayersBrickList.P1Bricks)
        {
            item.Kill();
        }
    }
#endif
}
