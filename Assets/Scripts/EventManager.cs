using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private LevelManager levelManager;
    private ScoreManager scoreManager = ScoreManager.instance;

    private void Awake()
    {
        if(instance != null )
        {
            Destroy(this);
            return;
        }

        instance = this;
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    public void OnPlayerDie(PlayerData player)
    {
        if( levelManager != null )
        {
            levelManager.DestroyPlayer(player);
        }

        if(scoreManager != null)
        {
            
        }

    }
}
