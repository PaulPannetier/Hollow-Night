using UnityEngine.SceneManagement;

public class TutorialManager : LevelManager
{

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
