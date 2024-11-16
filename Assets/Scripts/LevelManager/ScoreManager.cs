using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private int nbPlayer = 4;

    [Header("Score")]
    [SerializeField] private List<ScoreData> scoreDatas = new List<ScoreData>();


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        InitialiseScore();

    }

    private void InitialiseScore()
    {
        for (int i = 0; i < nbPlayer; i++)
        {
            scoreDatas.Add(new ScoreData((PlayerID)i, 0));
        }
    }

    public void AddScore(PlayerID playerID, int addScore)
    {
        foreach (ScoreData scoreData in scoreDatas)
        {
            if (scoreData.playerID == playerID)
            {
                scoreData.score += addScore;
                break;
            }
        }
    }


}

[System.Serializable]
public class ScoreData
{
    public PlayerID playerID;
    public int score;

    public ScoreData(PlayerID playerID, int score)
    {
        this.playerID = playerID;
        this.score = score;
    }
}

