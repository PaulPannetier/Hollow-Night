using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] players;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform endLevelPanel;


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
            EndGame();

    }

    private void InstantiatePlayers()
    {
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation, transform);
        }
    }

    private void InitialiseGame()
    {
        Time.timeScale = 1;
        endLevelPanel.gameObject.SetActive(false);

        InstantiatePlayers();
    }

    private bool CheckEndGame()
    {
        if (players.Count() > 0)
            return false;

        return true;
    }

    private void EndGame()
    {
        Time.timeScale = 0;

        if (endLevelPanel != null)
            endLevelPanel.gameObject.SetActive(true);
    }


}
