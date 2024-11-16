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
    private int nbOfGame;

    private bool isGameEnd = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);
        instance = this;
    }

    public void OnPlayerDie(PlayerData playerWhoDied, PlayerData killer)
    {
        LevelManager.instance.DestroyPlayer(playerWhoDied, killer);
    }

    public void InitialiseGame(int nbOfGame)
    {
        this.nbOfGame = nbOfGame;
        GenerateNextLevels();
        ScoreManager.instance.InitialiseScore();
        SwitchScene();
    }

    private void GenerateNextLevels()
    {
        nextLevels.Clear();
        if (levelList.Count == 0 || nbOfGame <= 0)
        {
            Debug.LogWarning("La liste des niveaux est vide ou `nbOfGame` est invalide.");
            return;
        }  List<string> _levelList = new List<string>(levelList);

        for (int i = 0; i < nbOfGame; i++)
        {
            if (_levelList.Count == 0)
            {
                _levelList = levelList;
            }
            string nextLevel = _levelList.GetRandom();
            _levelList.Remove(nextLevel);
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

        if (nextLevels.Count <= 0)
        {
            isGameEnd = true;
            LoadScene(rewardScene);
            return;
        }

        LoadScene(GetNextScene());

    }


    public void LoadScene(string scene)
    {
        AudioManager.instance.StopAllSound();
        SceneManager.LoadScene(scene);
    }


    private string GetNextScene()
    {
        string nextLevel = nextLevels[0];
        nextLevels.RemoveAt(0);
        return nextLevel;

    }
}
