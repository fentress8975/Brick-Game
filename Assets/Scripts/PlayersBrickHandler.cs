using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        if(m_PlayersBrickList.P1Bricks.Count == 0)
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
}
