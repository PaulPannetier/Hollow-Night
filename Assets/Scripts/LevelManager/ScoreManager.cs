using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private List<PlayerData> playerDatas = new List<PlayerData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

    }

    public void AddPlayerData(PlayerData data)
    {
        playerDatas.Add(data);
    }


}