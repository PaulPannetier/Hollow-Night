using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject boomFx;
    [SerializeField] private Transform boomPoint;
    [SerializeField] private float cooldown = 2f;

    private bool isReset;

    private void OnTriggerEnter(Collider other)
    {
        if (!isReset && other.CompareTag("Player"))
        {
            FaireBoom();
        }
    }

    private void FaireBoom()
    {
        StartCoroutine(ResetBoom());
        GameObject a = Instantiate(boomFx, boomPoint.position, boomPoint.rotation, transform);
        Destroy(a, 2f);
        AudioManager.instance.PlaySound("MineExplosion", 1f);
    }

    private IEnumerator ResetBoom()
    {
        isReset = true;
        yield return new WaitForSeconds(cooldown);
        isReset = false;
    }
}
