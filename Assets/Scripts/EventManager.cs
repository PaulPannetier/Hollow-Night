using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;


public class EventManager : MonoBehaviour
{
    public static EventManager instance;



    [Header("LevelData")]
    [SerializeField] private string rewardScene = "RewardScene";
    [SerializeField] private string menuScene = "MenuScene";
    [SerializeField] private List<string> levelList = new List<string>();
    private List<string> nextLevels = new List<string>();
    private int lvID;
    private int nbOfGame;

    private bool isGameEnd = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    public void OnPlayerDie(PlayerData player)
    {
        LevelManager.instance.DestroyPlayer(player);

    }

    public void InitialiseGame(int nbOfGame)
    {
        this.nbOfGame = nbOfGame;
        GenerateNextLevels();
    }

    private void GenerateNextLevels()
    {
        nextLevels.Clear();
        if (levelList.Count == 0 || nbOfGame <= 0)
        {
            Debug.LogWarning("La liste des niveaux est vide ou `nbOfGame` est invalide.");
            return;
        }

        for (int i = 0; i < nbOfGame; i++)
        {
            string nextLevel = levelList[Random.Rand(0, levelList.Count)];
            nextLevels.Add(nextLevel);
        }

        Debug.Log($"Liste des prochains niveaux générés : {string.Join(", ", nextLevels)}");
    }

    public void SwitchScene()
    {
        if (isGameEnd)
        {
            isGameEnd = false;
            LoadScene(menuScene);
            return;

        }

        if (nbOfGame <= 0)
        {
            isGameEnd = true;
            LoadScene(rewardScene);
            return;
        }

        LoadScene(GetNextScene());

    }


    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }


    private string GetNextScene()
    {
        string nextLevel = nextLevels[0];
        nextLevels.RemoveAt(0);
        return nextLevel;

    }
}
