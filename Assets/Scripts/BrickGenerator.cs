using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BrickGenerator : NetworkBehaviour
{
    [SerializeField] private GameObject m_BrickPrefab;

    public PlayersBricks GenerateSymmetrically(bool[,] pattern)
    {
        if (VerifiPattern(pattern))
        {
            if (IsHost)
            {
                List<Brick> p1 = GeneratePlayer1Area(pattern);
                List<Brick> p2 = GeneratePlayer2Area(pattern);
            }
            return new PlayersBricks(null , null);
        }
        else
        {
            throw new Exception("Неверный паттерн");
        }
    }

    private List<Brick> GeneratePlayer1Area(bool[,] pattern)
    {
        List<Brick> bricks = new List<Brick>();
        int _x = Player1BrickArea.Max_X;
        int _y = Player1BrickArea.Max_Y;

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                Vector3 _position = new(_x, _y, 0);
                GameObject go = Instantiate(m_BrickPrefab, Vector3.zero, m_BrickPrefab.transform.rotation);
                go.GetComponent<NetworkObject>().Spawn();
                _x++;
                _y--;
            }
        }
        return bricks;
    }

    private List<Brick> GeneratePlayer2Area(bool[,] pattern)
    {
        List<Brick> bricks = new List<Brick>();
        int _x = Player1BrickArea.Max_X;
        int _y = Player1BrickArea.Max_Y;

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                Vector3 _position = new(_x, _y, 0);
                GameObject go = Instantiate(m_BrickPrefab, Vector3.zero, m_BrickPrefab.transform.rotation);
                go.GetComponent<NetworkObject>().Spawn();
                _x--;
                _y--;
            }
        }
        return bricks;
    }

    private bool VerifiPattern(bool[,] pattern)
    {
        if (pattern.GetLength(0) <= Math.Abs(PlayerBrickArea.X) && pattern.GetLength(1) <= PlayerBrickArea.Y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public struct PlayerBrickArea
    {
        public const int X = 8;
        public const int Y = 6;
    }

    public struct Player1BrickArea
    {
        public const int Max_X = -8;
        public const int Max_Y = 4;
        public const int Min_X = -1;
        public const int Min_Y = -1;
    }

    public struct Player2BrickArea
    {
        public const int Max_X = 8;
        public const int Max_Y = 4;
        public const int Min_X = 1;
        public const int Min_Y = -1;
    }
}

public class PlayersBricks
{
    public PlayersBricks(List<Brick> player1, List<Brick> player2)
    {
        m_Player1Bricks = player1;
        m_Player2Bricks = player2;
    }

    public List<Brick> P1Bricks { get => m_Player1Bricks; }
    public List<Brick> P2Bricks { get => m_Player2Bricks; }

    private List<Brick> m_Player1Bricks;
    private List<Brick> m_Player2Bricks;
}

