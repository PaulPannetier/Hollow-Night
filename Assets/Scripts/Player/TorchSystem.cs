using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Collision2D;

public class TorchSystem : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private Light torchLight;
    [SerializeField] private Light spottedLight;
    private bool isNight => LevelManager.instance.isNight;
    [SerializeField] private LayerMask playerAndWallMask;
    private bool test;

    [SerializeField] private float coneAngle = 45f; // Angle du cône en degrés
    [SerializeField] private float rayDistance = 10f; // Distance maximale des rayons
    [SerializeField] private int rayCount = 10; // Nombre de rayons dans le cône

   

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAndWallMask = LayerMask.GetMask("Map", "Player");
    }

    void Start()
    {
        torchLight.enabled = false;
        spottedLight.enabled = false;
    }

    void Update()
    {
        if (playerInput.isTorchPressed && isNight)
        {
            torchLight.enabled = true;
            spottedLight.enabled = true;


            List<RaycastHit> hits = PerformConeRaycast();

            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.CompareTag("Player"))
                {
                    Debug.Log("Hit !!");
                    LevelManager.instance.DestroyPlayer(hit.transform.GetComponent<PlayerData>());
                }
            }
        }
        else if (playerInput.isTorchUp)
        {
            torchLight.enabled = false;
            spottedLight.enabled = false;
        }

    }

     public List<RaycastHit> PerformConeRaycast()
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        Vector3 origin = transform.position; // Origine des rayons
        Vector3 forward = transform.forward; // Direction centrale du cône

        for (int i = 0; i < rayCount; i++)
        {
            // Calcul d'un angle dans le cône
            float angle = Random.Rand(-coneAngle / 2f, coneAngle / 2f);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);

            // Calcul de la direction du rayon
            Vector3 rayDirection = rotation * forward;

            // Lancer le rayon
            if (Physics.Raycast(origin, rayDirection, out RaycastHit hit, rayDistance, playerAndWallMask))
            {
                hits.Add(hit);
            }

        }

        // Trier les hits par distance
        hits.Sort((hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        return hits;
    }
private void OnDrawGizmosSelected()
{
    // Origine des rayons (centre du joueur)
    Vector3 origin = transform.position; 

    // Direction centrale du cône
    Vector3 forward = transform.forward;

    // Dessiner les rayons dans le cône
    for (int i = 0; i < rayCount; i++)
    {
        // Calcul d'un angle dans le cône
        float angle = Random.Rand(-coneAngle / 2f, coneAngle / 2f);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        // Calcul de la direction du rayon
        Vector3 rayDirection = rotation * forward;

        // Visualiser le rayon dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, rayDirection * rayDistance);
    }
}
}
