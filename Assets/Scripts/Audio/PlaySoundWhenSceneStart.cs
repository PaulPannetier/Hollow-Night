using UnityEngine;

public class PlaySoundWhenSceneStart : MonoBehaviour
{
    [SerializeField] private bool stopAllSound = true;
    [SerializeField] private string[] soundToPlay;

    private void Start()
    {
        if(stopAllSound)
            AudioManager.instance.StopAllSound();

        foreach (string sound in soundToPlay)
        {
            AudioManager.instance.PlaySound(sound, 1f);
        }
    }
}
