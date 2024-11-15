using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject[] players;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<Transform> currentPlayers = new List<Transform>();
    private Transform lastPlayer;
    private ScoreManager scoreManager;

    [Header("EndLevelUI")]
    [SerializeField] private Transform endLevelPanel;
    [SerializeField] private TextMeshProUGUI winnerNameText;


    [Header("Lumiere")]
    [SerializeField] private Light dayLight;
    [SerializeField] private float minNextDay;
    [SerializeField] private float maxNextDay;

    [SerializeField] private float firstDayDuration = 5f;
    [SerializeField] private float dayDuration = 0.5f;
    [SerializeField] private float lightSpeedVariation = 5;
    [SerializeField] private float dayIntensity = 1;
    [SerializeField] private float nightIntensity = -1;

    private float lightTimer;
    private float targetLightIntensity = 1;

    public bool isNight;
    private bool isFirstDay;


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

        scoreManager = FindAnyObjectByType<ScoreManager>();
        InstantiatePlayers();
        SetFirstDay();
    }

    private void InstantiatePlayers()
    {
        for (int i = 0; i < players.Count(); i++)
        {
            GameObject newPlayer = Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation, transform);
            currentPlayers.Add(newPlayer.transform);
            newPlayer.GetComponent<PlayerInput>().controllerType = (ControllerType)i;

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
        if (dayLight == null)
        {
            Debug.Log("Pas de lumiere dans le levelManager");
            return;
        }


        lightTimer -= Time.deltaTime;

        if (lightTimer <= 0 && !isFirstDay)
        {
            StartCoroutine(SetDay(dayDuration));
        }

        dayLight.intensity = Mathf.Lerp(dayLight.intensity, targetLightIntensity, Time.deltaTime * lightSpeedVariation);

    }

    private void SetFirstDay()
    {
        isFirstDay = true;
        StartCoroutine(SetDay(firstDayDuration));
    }

    private IEnumerator SetDay(float dayDuration)
    {
        isNight = false;
        targetLightIntensity = dayIntensity;
        yield return new WaitForSeconds(dayDuration);
        targetLightIntensity = nightIntensity;
        isNight = true;
        ResetLightTimer();
    }


    private void ResetLightTimer()
    {
        isFirstDay = false;
        lightTimer = Random.Rand(minNextDay, maxNextDay);
    }

    #endregion

}
