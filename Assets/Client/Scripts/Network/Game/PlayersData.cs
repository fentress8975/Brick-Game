using UnityEngine;

public class PlayersData : SingletonMono<PlayersData>
{
    public Color Player1Color { get => m_Player1Color; set => m_Player1Color = value; }
    public string Player1Name { get => m_Player1Name; set => m_Player1Name = value; }
    public Color Player2Color { get => m_Player2Color; set => m_Player2Color = value; }
    public string Player2Name { get => m_Player2Name; set => m_Player2Name = value; }

    private Color m_Player1Color;
    private string m_Player1Name;
    private Color m_Player2Color;
    private string m_Player2Name;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SaveData(Color p1Color, Color p2Color, string p1Name, string p2Name)
    {
        m_Player1Color = p1Color;
        m_Player2Color = p2Color;
        m_Player1Name = p1Name;
        m_Player2Name = p2Name;
    }
}
