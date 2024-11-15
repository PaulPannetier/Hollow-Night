using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject[] players;

    [SerializeField] private Transform[] spawnPoints;
    private List<Transform> currentPlayers = new List<Transform>();
    private Transform lastPlayer;

    [Header("EndLevelUI")]
    [SerializeField] private Transform endLevelPanel;
    [SerializeField] private TextMeshProUGUI winnerNameText;


    [Header("Lumiere")]
    [SerializeField] private Light dayLight;
    [SerializeField] private float minNextDay;
    [SerializeField] private float maxNextDay;

    private float lightTimer;

    private bool Error => players.Count() > spawnPoints.Count();


    void Start()
    {
        if (Error)
            Debug.LogWarning("pas assez de spawnPoints pour les joueurs");

        InitialiseGame();
    }

    void Update()
    {
        if (CheckEndGame())
        {
            EndGame();
        }

        UpdateLight();
    }

    #region Initialise Game
    private void InitialiseGame()
    {
        Time.timeScale = 1;
        lastPlayer = null;
        endLevelPanel.gameObject.SetActive(false);

        InstantiatePlayers();
    }

    private void InstantiatePlayers()
    {
        for (int i = 0; i < players.Count(); i++)
        {
            GameObject newPlayer = Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation, transform);
            currentPlayers.Add(newPlayer.transform);
        }
    }

    #endregion

    #region EndGame Region
    private bool CheckEndGame()
    {
        if (currentPlayers.Count() > 1)
        {
            return false;
        }

        lastPlayer = currentPlayers[0];
        return true;
    }

    private void EndGame()
    {
        Time.timeScale = 0;

        if (endLevelPanel != null)
        {
            endLevelPanel.gameObject.SetActive(true);

            if (lastPlayer != null && winnerNameText != null)
                winnerNameText.text = lastPlayer.name;


        }
    }
    #endregion

    public void DestroyPlayer(Transform player)
    {
        if (currentPlayers.Contains(player))
        {
            currentPlayers.Remove(player);
            Destroy(player.gameObject, 2f);
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    #region Light
    private void UpdateLight()
    {
        lightTimer += Time.deltaTime;


    }

    private void SetDay()
    {

    }

    #endregion

}
