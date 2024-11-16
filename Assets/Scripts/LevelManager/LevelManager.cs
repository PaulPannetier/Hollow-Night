using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Player")]
    [SerializeField] private GameObject[] players;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerDeathFx;
    [SerializeField] private List<PlayerData> currentPlayers = new List<PlayerData>();

    private Transform lastPlayer;

    [Header("EndLevelUI")]
    [SerializeField] private Transform endLevelPanel;
    [SerializeField] private TextMeshProUGUI winnerNameText;
    [SerializeField] private float timeBeforeScorePanel = 10f;
    [SerializeField] private GameObject endLevelFx;
    [SerializeField] private Transform fxPointsList;
    private Transform[] fxPoints;


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
    private bool isInstantiating = false;

    public bool isLevelRunning = false;
    private bool Error => players.Count() > spawnPoints.Count();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Start()
    {
        if (Error)
            Debug.LogWarning("pas assez de spawnPoints pour les joueurs");

        InitialiseLevel();
    }

    void Update()
    {
        if (isLevelRunning && CheckEndGame())
        {
            EndGame();
        }

        UpdateLight();
    }

    #region Initialise Game
    private void InitialiseLevel()
    {
        isLevelRunning = true;
        lastPlayer = null;
        endLevelPanel.gameObject.SetActive(false);

        InstantiatePlayers();
        SetFirstDay();

        //fx 
        if (fxPointsList != null)
        {
            int childCount = fxPointsList.childCount; // Nombre d'enfants
            fxPoints = new Transform[childCount]; // Initialiser le tableau

            for (int i = 0; i < childCount; i++)
            {
                fxPoints[i] = fxPointsList.GetChild(i); // Ajouter chaque enfant
            }
        }
        else
        {
            Debug.LogWarning("fxPointsList n'est pas assigné dans l'inspecteur !");
        }

    }

    private void InstantiatePlayers()
    {
        for (int i = 0; i < players.Count(); i++)
        {
            GameObject newPlayer = Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation, transform);
            newPlayer.GetComponent<PlayerInput>().controllerType = (ControllerType)i;
            currentPlayers.Add(newPlayer.GetComponent<PlayerData>());

            PlayerData newPlayerData = newPlayer.GetComponent<PlayerData>();
            if (newPlayerData != null)
            {
                newPlayerData.playerID = (PlayerID)i;
            }

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

        lastPlayer = currentPlayers[0].transform;
        return true;
    }

    private void EndGame()
    {
        isLevelRunning = false;

        AddScore();

        if (endLevelPanel != null)
        {
            endLevelPanel.gameObject.SetActive(true);

            if (lastPlayer != null && winnerNameText != null)
                winnerNameText.text = "Le Vainqueur est :\n" + lastPlayer.GetComponent<PlayerData>().playerID;


        }

        CheckEndLevelFx();
        StartCoroutine(DisplayScorePanel());

    }

    private void AddScore()
    {

        PlayerData lastPlayerData = lastPlayer.GetComponent<PlayerData>();
        ScoreManager.instance.AddScore(lastPlayerData.playerID, 1);
    }

    private IEnumerator DisplayScorePanel()
    {
        yield return new WaitForSeconds(timeBeforeScorePanel);
        ScoreManager.instance.DisplayScorePanel();
    }
    #endregion

    public void DestroyPlayer(PlayerData playerWhoDied, PlayerData killer)
    {
        if (currentPlayers.Contains(playerWhoDied))
        {
            print("oui");
            currentPlayers.Remove(playerWhoDied);
            Instantiate(playerDeathFx, playerWhoDied.transform.position, playerWhoDied.transform.rotation,transform);
            Destroy(playerWhoDied.gameObject, 0f);
        }

        killer.nbKill++;
        ScoreManager.instance.AddKillToScore(killer.playerID);
    }

    #region Fx
    private void CheckEndLevelFx()
    {
        if (fxPoints.Length > 0 && !isLevelRunning && !isInstantiating)
        {
            StartCoroutine(InstantiateEndLevelFx());
        }
    }

    private IEnumerator InstantiateEndLevelFx()
    {
        isInstantiating = true; // Empêche d'autres appels pendant l'exécution

        int fxCount = 20; // Nombre total d'effets à instancier
        float interval = 0.5f; // Intervalle entre chaque effet

        for (int i = 0; i < fxCount; i++)
        {
            // Choisir un point aléatoire
            int randomIndex = Random.Rand(0, fxPoints.Length - 1);
            Vector3 spawnPosition = fxPoints[randomIndex].position;

            // Instancier l'effet
            Instantiate(endLevelFx, spawnPosition, Quaternion.identity);

            // Attendre avant de passer au suivant
            yield return new WaitForSeconds(interval);
        }

        isInstantiating = false; // Fin de l'instantiation
    }

    #endregion

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
