using UnityEngine;

public class LampRefill : LightInteractable
{
    private new Light light;

    [SerializeField, Range(0f, 1f)] private float startAmount;
    [SerializeField] private float minIntensity, maxIntensity;
    [SerializeField] private float durationToRefill;
    [SerializeField] private float durationToDefill;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void Start()
    {
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, startAmount);
    }

    private void Update()
    {
        if(isActive)
        {
            float amountToRegen = (maxIntensity - minIntensity) / durationToRefill;
            light.intensity = Mathf.Min(maxIntensity, light.intensity + (amountToRegen * Time.deltaTime));
        }
        else
        {
            float amountToRegen = (maxIntensity - minIntensity) / durationToDefill;
            light.intensity = Mathf.Min(maxIntensity, light.intensity + (amountToRegen * Time.deltaTime));
        }
    }

    #region OnValidate

#if UNITY_EDITOR

    private void OnValidate()
    {
        maxIntensity = Mathf.Max(0f, maxIntensity);
        minIntensity = Mathf.Max(0f, minIntensity);
    }

#endif

    #endregion
}
