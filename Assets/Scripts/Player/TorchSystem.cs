using UnityEngine;
using System.Collections.Generic;

public class TorchSystem : MonoBehaviour
{
#if UNITY_EDITOR
    private bool drawGizmos = true;
#endif

    private PlayerInput playerInput;
    private PlayerData playerData;
    private bool isNight => LevelManager.instance.isNight;
    private float lightRange => isPuttingAHat ? 2f * torchLight.range : torchLight.range;
    private float coneAngle => isPuttingAHat ? 1.1f * torchLight.spotAngle : torchLight.spotAngle;
    private List<EnemiData> enemies;
    private HashSet<LightInteractable> lightInteractables;
    private LayerMask playerAndWallMask, lightInteractableMask;
    private bool isPuttingAHat;
    private float baseTorchLightIntensity, baseTorchSpotAngle;

    [SerializeField] private Light torchLight;
    [SerializeField] private Light spottedLight;
    [SerializeField] private int rayCount = 10;
    [SerializeField] private float durationToKill = 2f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAndWallMask = LayerMask.GetMask("Map", "Player");
        lightInteractableMask = LayerMask.GetMask("LightInteractable", "Map");
        enemies = new List<EnemiData>(4);
        playerData = GetComponent<PlayerData>();
        lightInteractables = new HashSet<LightInteractable>();
    }

    private void Start()
    {
        torchLight.enabled = false;
        spottedLight.enabled = false;
        baseTorchLightIntensity = torchLight.intensity;
        baseTorchSpotAngle = torchLight.spotAngle;
    }

    private void Update()
    {
        if (isNight)
        {
            if(playerInput.isTorchDown)
            {
                AudioManager.instance.PlaySound("TorchUp", 1f);
            }

            if (playerInput.isTorchPressed)
            {
                torchLight.enabled = true;
                spottedLight.enabled = true;

                HashSet<LightInteractable> lightInteractables = GetLightInteractable();
                foreach (LightInteractable ligthInter in lightInteractables)
                {
                    if(!this.lightInteractables.Contains(ligthInter))
                    {
                        ligthInter.BeginInteract(gameObject);
                    }
                }

                foreach (LightInteractable ligthInter in this.lightInteractables)
                {
                    if (!lightInteractables.Contains(ligthInter))
                    {
                        ligthInter.EndInteract(gameObject);
                    }
                }
                this.lightInteractables.Clear();

                this.lightInteractables = lightInteractables;

                List<Collider> cols = GetPlayerInTorch();
                bool[] found = new bool[enemies.Count];
                bool[] colsAlreadyTouch = new bool[cols.Count];

                for (int j = 0; j < cols.Count; j++)
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (cols[j].gameObject == enemies[i].enemiGO)
                        {
                            colsAlreadyTouch[j] = true;
                            found[i] = true;
                            break;
                        }
                    }
                }

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (!found[i])
                    {
                        enemies[i].enemiGO.GetComponent<PlayerEvent>().OnPlayerEndLightUp(gameObject);
                        enemies.RemoveAt(i);
                        continue;
                    }

                    if (Time.time - enemies[i].firtsTimeTouch > durationToKill)
                    {
                        enemies[i].enemiGO.GetComponent<PlayerEvent>().OnPlayerEndLightUp(gameObject);
                        PlayerData enemiData = enemies[i].enemiGO.GetComponent<PlayerData>();
                        EventManager.instance.OnPlayerDie(enemiData, playerData);
                        enemies.RemoveAt(i);
                    }
                }

                for (int i = 0; i < cols.Count; i++)
                {
                    if (!colsAlreadyTouch[i])
                    {
                        enemies.Add(new EnemiData(Time.time, cols[i].gameObject));
                        cols[i].GetComponent<PlayerEvent>().OnPlayerLightUp(gameObject);
                    }
                }
            }
            else if (playerInput.isTorchUp)
            {
                StopTorch();
            }
        }
        else
        {
            StopTorch();
        }
    }

    public void OnPutHat()
    {
        isPuttingAHat = true;
        torchLight.intensity = 3f * baseTorchLightIntensity;
        torchLight.spotAngle = 1.1f * baseTorchSpotAngle;
    }

    public void OnRealeaseHat()
    {
        isPuttingAHat = false;
        torchLight.intensity = baseTorchLightIntensity;
        torchLight.spotAngle = baseTorchSpotAngle;
    }

    private void StopTorch()
    {
        torchLight.enabled = false;
        spottedLight.enabled = false;
        enemies.Clear();

        foreach (LightInteractable li in lightInteractables)
        {
            li.EndInteract(gameObject);
        }
        lightInteractables.Clear();
    }

    private List<Collider> GetPlayerInTorch()
    {
        RaycastHit[] hits = new RaycastHit[rayCount];

        float angleStep = (Mathf.PI * 2f) / (rayCount - 1);
        float radius = Mathf.Tan(coneAngle) * lightRange;
        Vector3 center = torchLight.transform.position + torchLight.transform.forward * lightRange;

        for (int i = 0; i < rayCount - 1; i++)
        {
            float angle = i * angleStep;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            Vector3 offset = x * torchLight.transform.right + y * torchLight.transform.up;
            Vector3 rayDirection = ((center + offset) - torchLight.transform.position).normalized;

            Physics.Raycast(torchLight.transform.position, rayDirection, out hits[i], lightRange, playerAndWallMask);
        }

        Physics.Raycast(torchLight.transform.position, torchLight.transform.forward, out hits[rayCount - 1], lightRange, playerAndWallMask);

        HashSet<Collider> cols = new HashSet<Collider>(4);

        for (int i = 0; i < rayCount; i++)
        {
            if(hits[i].collider != null && hits[i].collider.CompareTag("Player") && hits[i].collider.gameObject != gameObject)
            {
                cols.Add(hits[i].collider);
            }
        }

        List<Collider> res = new List<Collider>(cols);
        return res;
    }

    private HashSet<LightInteractable> GetLightInteractable()
    {
        RaycastHit[] hits = new RaycastHit[rayCount];

        float angleStep = (Mathf.PI * 2f) / (rayCount - 1);
        float radius = Mathf.Tan(coneAngle) * lightRange;
        Vector3 center = torchLight.transform.position + torchLight.transform.forward * lightRange;

        for (int i = 0; i < rayCount - 1; i++)
        {
            float angle = i * angleStep;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            Vector3 offset = x * torchLight.transform.right + y * torchLight.transform.up;
            Vector3 rayDirection = ((center + offset) - torchLight.transform.position).normalized;

            Physics.Raycast(torchLight.transform.position, rayDirection, out hits[i], lightRange, lightInteractableMask);
        }

        HashSet<LightInteractable> lightInteractables = new HashSet<LightInteractable>(hits.Length);
        int lightMask = LayerMask.GetMask("LightInteractable");
        for (int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider != null)
            {
                LightInteractable lightInteractable = hits[i].collider.GetComponent<LightInteractable>();
                if (lightInteractable != null)
                {
                    lightInteractables.Add(lightInteractable);
                }
            }
        }

        return lightInteractables;
    }

    #region OnValidate/Gizmos

#if UNITY_EDITOR

    private void OnValidate()
    {
        rayCount = Mathf.Max(rayCount, 1);
        durationToKill = Mathf.Max(durationToKill, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.red;
        float angleStep = (Mathf.PI * 2f) / rayCount;
        float radius = Mathf.Tan(coneAngle * 0.5f * Mathf.Deg2Rad) * lightRange;

        Vector3 center = torchLight.transform.position + torchLight.transform.forward * lightRange;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            Vector3 offset = x * torchLight.transform.right + y * torchLight.transform.up;
            Vector3 rayDirection = ((center + offset) - torchLight.transform.position).normalized;

            Gizmos.DrawLine(torchLight.transform.position, torchLight.transform.position + rayDirection * lightRange);
        }
    }

#endif

    #endregion

    #region Custom Struct

    private class EnemiData
    {
        public float firtsTimeTouch;
        public GameObject enemiGO;

        public EnemiData(float firtsTimeTouch, GameObject enemiGO)
        {
            this.firtsTimeTouch = firtsTimeTouch;
            this.enemiGO = enemiGO;
        }
    }

    #endregion
}
