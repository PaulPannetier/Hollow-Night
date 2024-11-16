using UnityEngine;

public class LampRefill : LightInteractable
{
    private GameObject playerToKill;
    private float lastTimeStartKillPlayer;
    private LayerMask playerMask;

#if UNITY_EDITOR
    [SerializeField] private bool autoUpdate;
#endif

    [SerializeField] private new Light light;
    [SerializeField, Range(0f, 1f)] private float startAmount;
    [SerializeField] private float minIntensity, maxIntensity, minIntensityToKill;
    [SerializeField] private float durationToRefill;
    [SerializeField] private float durationToDefill;
    [SerializeField] private float killRadius;
    [SerializeField] private float killDuration;
    [SerializeField] private Transform fire;

    private void Awake()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, startAmount);
    }

    private void Update()
    {
        if (isActive)
        {
            float amountToRegen = (maxIntensity - minIntensity) / durationToRefill;
            light.intensity = Mathf.Min(maxIntensity, light.intensity + (amountToRegen * Time.deltaTime));
        }
        else
        {
            float amountToRegen = (maxIntensity - minIntensity) / durationToDefill;
            light.intensity = Mathf.Max(minIntensity, light.intensity - (amountToRegen * Time.deltaTime));
        }

        fire.gameObject.SetActive(light.intensity >= minIntensityToKill);

        if (light.intensity >= minIntensityToKill && lastActivator != null)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, killRadius, playerMask);

            bool foundPlayerToKill = false;
            if(playerToKill != null)
            {
                foreach (Collider col in cols)
                {
                    if (col.CompareTag("Player") && col.gameObject == playerToKill)
                    {
                        foundPlayerToKill=true;
                        if(Time.time - lastTimeStartKillPlayer > killDuration)
                        {
                            KillPlayer();
                        }
                        break;
                    }
                }
            }

            if (foundPlayerToKill)
                return;

            foreach (Collider col in cols)
            {
                if (col.CompareTag("Player") && col.gameObject != lastActivator)
                {
                    lastTimeStartKillPlayer = Time.time;
                    playerToKill = col.gameObject;
                    break;
                }
            }
        }
        else
        {
            lastTimeStartKillPlayer = -10f;
            playerToKill = null;
        }
    }

    private void KillPlayer()
    {
        PlayerData playerWhoKill = playerToKill.GetComponent<PlayerData>();
        PlayerData killer = lastActivator.GetComponent<PlayerData>();
        EventManager.instance.OnPlayerDie(playerWhoKill, killer);
        lastTimeStartKillPlayer = -10f;
        playerToKill = null;
    }

    #region OnValidate/Gizmos

#if UNITY_EDITOR

    private void OnValidate()
    {
        maxIntensity = Mathf.Max(0f, maxIntensity);
        minIntensity = Mathf.Max(0f, minIntensity);
        killRadius = Mathf.Max(0f, killRadius);
        killDuration = Mathf.Max(0f, killDuration);
        if(autoUpdate)
        {
            light.intensity = minIntensity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);
    }

#endif

    #endregion
}
