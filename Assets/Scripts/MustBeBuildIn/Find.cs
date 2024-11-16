using UnityEngine;

public class Find : MonoBehaviour
{
    [SerializeField] private bool find;
    [SerializeField] private AudioSource[] res;

    private void OnValidate()
    {
        if(find)
        {
            find = false;
            res = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        }
    }
}
