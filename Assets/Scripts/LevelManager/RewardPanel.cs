using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] private string menuScene = "MenuScene";

    [Header("UI")]
    [SerializeField] private Transform killerPanel;
    [SerializeField] private TextMeshProUGUI killerText;
    [SerializeField] private Transform poulpePanel;
    [SerializeField] private TextMeshProUGUI poulpeText;
    [SerializeField] private Transform survivorPanel;
    [SerializeField] private TextMeshProUGUI survivorText;
    [SerializeField] private Transform PoissonGlobePanel;
    [SerializeField] private TextMeshProUGUI poissonText;

    [Header("Recompense")]
    [SerializeField] private PlayerID killerPlayer; // to do  
    [SerializeField] private PlayerID survivorPlayer;
    [SerializeField] private PlayerID poulpePlayer;
    [SerializeField] private PlayerID poissonPlayer;

    [SerializeField] private List<ScoreData> scoreDatas = new List<ScoreData>();

    private enum PanelState
    {
        None,
        Killer,
        Poulpe,
        Survivor,
        PoissonGlobe
    }

    private PanelState currentState = PanelState.None;
    private void Awake()
    {
        scoreDatas = ScoreManager.instance.scoreDatas;
        SetReward();
    }
    void Start()
    {
        SwitchPanel(PanelState.Killer);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CyclePanels();
        }
    }



    private void CyclePanels()
    {
        switch (currentState)
        {
            case PanelState.None:
                SwitchPanel(PanelState.Killer);
                break;
            case PanelState.Killer:
                SwitchPanel(PanelState.Poulpe);
                break;
            case PanelState.Poulpe:
                SwitchPanel(PanelState.Survivor);
                break;
            case PanelState.Survivor:
                SwitchPanel(PanelState.PoissonGlobe);
                break;
            case PanelState.PoissonGlobe:
                SwitchPanel(PanelState.None);
                break;
        }
    }
    private void SwitchPanel(PanelState newState)
    {
        if (currentState == newState)
            return;

        DiscardAllPanels();

        currentState = newState;
        switch (newState)
        {
            case PanelState.Killer:
                killerPanel.gameObject.SetActive(true);
                break;
            case PanelState.Poulpe:
                poulpePanel.gameObject.SetActive(true);
                break;
            case PanelState.Survivor:
                survivorPanel.gameObject.SetActive(true);
                break;
            case PanelState.PoissonGlobe:
                PoissonGlobePanel.gameObject.SetActive(true);
                break;
            case PanelState.None:
                ResetGame();
                break;
        }
    }

    private void DiscardAllPanels()
    {
        killerPanel.gameObject.SetActive(false);
        poulpePanel.gameObject.SetActive(false);
        survivorPanel.gameObject.SetActive(false);
        PoissonGlobePanel.gameObject.SetActive(false);
    }

    private void SetReward()
    {
        killerPlayer = SetKiller();
        survivorPlayer = SetSurvivor();
        poulpePlayer = SetPoulpe();
        poissonPlayer = SetPoisson();

        DisplayReward();
    }

    PlayerID SetKiller()
    {
        int hightestKill = -1;
        ScoreData currentKiller = new ScoreData(PlayerID.Player1, 0);

        foreach (ScoreData score in scoreDatas)
        {
            if (score.nbKill > hightestKill)
            {
                hightestKill = score.nbKill;
                currentKiller = score;
            }
        }

        scoreDatas.Remove(currentKiller);
        return currentKiller.playerID;
    }

    PlayerID SetSurvivor()
    {
        int hightestScore = -1;
        ScoreData currentSurvivor = new ScoreData(PlayerID.Player1, 0);
        foreach (ScoreData score in scoreDatas)
        {
            if (score.score > hightestScore)
            {
                hightestScore = score.score;
                currentSurvivor = score;
            }
        }
        scoreDatas.Remove(currentSurvivor);
        return currentSurvivor.playerID;
    }

    PlayerID SetPoulpe()
    {
        return scoreDatas[Random.Rand(0, 1)].playerID;
    }

    PlayerID SetPoisson()
    {
        return scoreDatas[0].playerID;
    }

    void DisplayReward()
    {
        killerText.text = killerPlayer.ToString();
        survivorText.text = survivorPlayer.ToString();
        poulpeText.text = poulpePlayer.ToString();
        poissonText.text = poissonPlayer.ToString();
    }
    void ResetGame()
    {
        EventManager.instance = null;
        ScoreManager.instance = null;

        SceneManager.LoadScene(menuScene);
    }
}
