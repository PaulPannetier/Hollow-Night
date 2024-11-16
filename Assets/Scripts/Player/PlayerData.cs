using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public PlayerID playerID;
    [HideInInspector] public int nbKill;
    public Transform hatTransform;
    public GameObject hat;
}

public enum PlayerID{
    Player1,
    Player2,
    Player3,
    Player4
}
