using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayersBrickHandler : NetworkBehaviour
{
    public Action OnPlayer1Victory;
    public Action OnPlayer2Victory;

    [SerializeField] private BrickGenerator m_BrickGenerator;
    private PlayersBricks m_PlayersBrickList;

    public void GeneratePlayerAreasSymmetrical(bool[,] pattern)
    {
        if (IsHost)
        {
            m_PlayersBrickList = m_BrickGenerator.GenerateSymmetrically(pattern);
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

    private void CheckVictoryConditionP1()
    {
        if (m_PlayersBrickList.P1Bricks.Count == 0)
        {
            OnPlayer1Victory?.Invoke();
        }
    }

    private void CheckVictoryConditionP2()
    {
        if (m_PlayersBrickList.P2Bricks.Count == 0)
        {
            OnPlayer2Victory?.Invoke();
        }
    }

    [ContextMenu("GenerateDebug")]
    private void GenerateArea()
    {
        bool[,] bools = new bool[6, 8];
        for (int i = 0; i < bools.GetLength(0); i++)
        {
            for (int j = 0; j < bools.GetLength(1); j++)
            {
                bools[i, j] = UnityEngine.Random.value > 0.5f;
            }
        }
        GeneratePlayerAreasSymmetrical(bools);
    }
}
