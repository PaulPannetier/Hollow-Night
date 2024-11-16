using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private int nbPlayer = 4;

    [Header("Score")]
    [SerializeField] private List<ScoreData> scoreDatas = new List<ScoreData>();
    [SerializeField] private GameObject ScoreCanva;
    [SerializeField] private TextMeshProUGUI scoreTextPrefab;

    private GameObject currentScorePanel;

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

        HideScorePanel();
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

    public void DisplayScorePanel()
    {
        currentScorePanel = Instantiate(ScoreCanva, transform);
        GetComponentInChildren<Button>().onClick.AddListener(HideScorePanel);
        UpdateScorePanel();
    }

    public void HideScorePanel()
    {
        if (currentScorePanel != null)
        {
            Destroy(currentScorePanel);
        }
    }

    private void UpdateScorePanel()
    {

        for (int i = 0; i < nbPlayer; i++)
        {
            var scoreText = Instantiate(scoreTextPrefab, currentScorePanel.GetComponentInChildren<HorizontalLayoutGroup>().transform);
            if (scoreText != null)
            {
                scoreText.text = "   " + scoreDatas[i].playerID.ToString() + "   " + "\n" + scoreDatas[i].score.ToString();
            }
            else
            {
                Debug.LogWarning("pas de text pour afficher le score");
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

