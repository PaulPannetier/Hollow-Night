using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public PlayerID playerID;
    public int nbKill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum PlayerID{
    Player1,
    Player2,
    Player3,
    Player4
}
