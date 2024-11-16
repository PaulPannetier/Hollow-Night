using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Transform mainsPanel;
    [SerializeField] private Transform nbOfLevel;

    [SerializeField] private string tutorialName = "Tutorial";

    private void Start()
    {
        mainsPanel.gameObject.SetActive(true);
        nbOfLevel.gameObject.SetActive(false);
    }
    public void PlayGame()
    {
        nbOfLevel.gameObject.SetActive(true);
        mainsPanel.gameObject.SetActive(false);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainPanel()
    {
        mainsPanel.gameObject.SetActive(true);
        nbOfLevel.gameObject.SetActive(false);
    }

    public void LaunchTutorial()
    {
        SceneManager.LoadScene(tutorialName);
    }

    public void InitialiseGame(int nbOfGame)
    {
        EventManager.instance.InitialiseGame(nbOfGame);
    }

}

