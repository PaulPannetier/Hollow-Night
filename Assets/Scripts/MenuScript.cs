using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public void playGame()
    {
        SceneManager.LoadScene("StageChooseScene");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void loadScene(string sceneName)
    {
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
