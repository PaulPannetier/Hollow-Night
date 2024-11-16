using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StarWarsCredits : MonoBehaviour
{
    [SerializeField] private RectTransform creditsText; // Référence au texte des crédits
    [SerializeField] private float scrollSpeed = 50f; // Vitesse de défilement
    [SerializeField] private float resetDelay = 5f;
    [SerializeField] private string menuScene = "MenuScene";


    void Start()
    {
        if (creditsText == null)
        {
            Debug.LogError("Aucun RectTransform assigné au script !");
            return;
        }

    }

    void Update()
    {
        if (creditsText != null)
        {
            // Défilement du texte vers le haut
            creditsText.localPosition += Vector3.up * scrollSpeed * Time.deltaTime;

            // Réinitialisation si le texte dépasse un certain point
            if (creditsText.localPosition.y > Screen.height + creditsText.rect.height)
            {
                StartCoroutine(ResetCredits());
            }
        }
    }

    private IEnumerator ResetCredits()
    {
        yield return new WaitForSeconds(resetDelay);
        SceneManager.LoadScene(menuScene);
    }
}
