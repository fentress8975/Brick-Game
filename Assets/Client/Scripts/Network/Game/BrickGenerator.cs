using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BrickGenerator : NetworkBehaviour
{
    [SerializeField] private Brick m_BrickPrefab;
    [SerializeField] private BrickClient m_BrickClientPrefab;
    [SerializeField] private Transform m_BrickServerParent;
    [SerializeField] private Transform m_BrickParentClient;
    [SerializeField] private Material m_Player1Color;
    [SerializeField] private Material m_Player2Color;


    public PlayersBricks GenerateSymmetrically(bool[,] pattern)
    {
        if (VerifiPattern(pattern))
        {
            if (IsHost)
            {
                Color p1Color = GameNetworkHandler.Singletone.P1Color;
                Color p2Color = GameNetworkHandler.Singletone.P2Color;
                m_Player1Color.color = p1Color;
                m_Player2Color.color = p2Color;
                LoadPlayersColorClientRpc(p1Color.r, p1Color.g, p1Color.b, p2Color.r, p2Color.g, p2Color.b);
                List<Brick> p1 = GeneratePlayer1Area(pattern);
                List<Brick> p2 = GeneratePlayer2Area(pattern);
                return new PlayersBricks(p1, p2);
            }
            else
            {
                return new PlayersBricks(null, null);
            }
        }
        else
        {
            throw new Exception("Неверный паттерн");
        }
    }



    internal PlayersBricksClient GenerateSymmetricallyClient(bool[,] brickListPattern, ulong playerID)
    {
        if (playerID == 1)
        {
            List<BrickClient> p1 = GeneratePlayer1AreaClient(brickListPattern);
            return new PlayersBricksClient(p1, null);
        }
        else
        {
            List<BrickClient> p2 = GeneratePlayer2AreaClient(brickListPattern);
            return new PlayersBricksClient(null, p2);
        }
    }

    private List<BrickClient> GeneratePlayer1AreaClient(bool[,] pattern)
    {
        List<BrickClient> bricks = new List<BrickClient>();
        int _x = Player1BrickArea.Max_X;
        int _y = Player1BrickArea.Max_Y;

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                if (pattern[i, j])
                {
                    Vector3 _position = new(_x, _y, 0);
                    BrickClient brick = Instantiate(m_BrickClientPrefab, _position, m_BrickClientPrefab.transform.rotation);
                    brick.gameObject.name = $"P1 {i}{j}";
                    brick.transform.SetParent(m_BrickParentClient);
                    bricks.Add(brick);
                }
                _x++;
            }
            _y--;
            _x = Player1BrickArea.Max_X;
        }
        return bricks;
    }

    private List<BrickClient> GeneratePlayer2AreaClient(bool[,] pattern)
    {
        List<BrickClient> bricks = new List<BrickClient>();
        int _x = Player2BrickArea.Max_X;
        int _y = Player2BrickArea.Max_Y;

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                if (pattern[i, j])
                {
                    Vector3 _position = new(_x, _y, 0);
                    BrickClient brick = Instantiate(m_BrickClientPrefab, _position, m_BrickClientPrefab.transform.rotation);
                    brick.gameObject.name = $"P2 {i}{j}";
                    brick.transform.SetParent(m_BrickParentClient);
                    bricks.Add(brick);
                }
                _x--;
            }
            _y--;
            _x = Player2BrickArea.Max_X;
        }
        return bricks;
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
                if (pattern[i, j])
                {
                    Vector3 _position = new(_x, _y, 0);
                    Brick brick = Instantiate(m_BrickPrefab, _position, m_BrickPrefab.transform.rotation);
                    brick.gameObject.name = $"P1 {i}{j}";
                    brick.GetComponent<NetworkObject>().Spawn();
                    brick.GetComponent<NetworkObject>().TrySetParent(m_BrickServerParent);
                    brick.ChangeColorClientRpc(Brick.PLAYER1);
                    bricks.Add(brick);
                }
                _x++;
            }
            _y--;
            _x = Player1BrickArea.Max_X;
        }
        return bricks;
    }

    private List<Brick> GeneratePlayer2Area(bool[,] pattern)
    {
        List<Brick> bricks = new List<Brick>();
        int _x = Player2BrickArea.Max_X;
        int _y = Player2BrickArea.Max_Y;

        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                if (pattern[i, j])
                {
                    Vector3 _position = new(_x, _y, 0);
                    Brick brick = Instantiate(m_BrickPrefab, _position, m_BrickPrefab.transform.rotation);
                    brick.gameObject.name = $"P2 {i}{j}";
                    brick.GetComponent<NetworkObject>().Spawn();
                    brick.GetComponent<NetworkObject>().TrySetParent(m_BrickServerParent);
                    brick.ChangeColorClientRpc(Brick.PLAYER2);
                    bricks.Add(brick);
                }
                _x--;
            }
            _y--;
            _x = Player2BrickArea.Max_X;
        }
        return bricks;
    }

    private bool VerifiPattern(bool[,] pattern)
    {
        if (pattern.GetLength(0) <= Math.Abs(PlayerBrickArea.Y) && pattern.GetLength(1) <= PlayerBrickArea.X)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [ClientRpc]
    private void LoadPlayersColorClientRpc(float p1Red, float p1Green, float p1Blue, float p2Red, float p2Green, float p2Blue)
    {
        m_Player1Color.color = new(p1Red, p1Green, p1Blue);
        m_Player2Color.color = new(p2Red, p2Green, p2Blue);
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

public class PlayersBricksClient
{
    public PlayersBricksClient(List<BrickClient> player1, List<BrickClient> player2)
    {
        m_Player1Bricks = player1;
        m_Player2Bricks = player2;
    }

    public List<BrickClient> P1Bricks { get => m_Player1Bricks; }
    public List<BrickClient> P2Bricks { get => m_Player2Bricks; }

    private List<BrickClient> m_Player1Bricks;
    private List<BrickClient> m_Player2Bricks;
}