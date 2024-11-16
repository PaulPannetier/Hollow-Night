using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Transform mainsPanel;
    [SerializeField] private Transform nbOfLevel;

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

}

