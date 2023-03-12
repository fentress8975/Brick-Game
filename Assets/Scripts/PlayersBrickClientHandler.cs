using System.Collections.Generic;
using UnityEngine;


public class PlayersBrickClientHandler : MonoBehaviour
{
    public PlayersBricksClient BrickList { get; }

    private PlayersBricksClient m_PlayersBrickList;
    [SerializeField] private BrickGenerator m_BrickGenerator;
    private ulong m_PlayerID;

    public void SetBrickList(bool[,] brickListPattern, ulong id)
    {
        m_PlayerID = id;
        ClearBricks();
        m_BrickGenerator.GenerateSymmetricallyClient(brickListPattern, m_PlayerID);
    }

    private void ClearBricks()
    {
        if (m_PlayersBrickList != null)
        {
            if (m_PlayersBrickList.P1Bricks != null)
            {
                foreach (var item in m_PlayersBrickList.P1Bricks)
                {
                    Destroy(item.gameObject);
                }
            }
            if (m_PlayersBrickList.P2Bricks != null)
            {
                foreach (var item in m_PlayersBrickList.P2Bricks)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
}
