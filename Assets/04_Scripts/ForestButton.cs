using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class ForestButton : MonoBehaviour
{
    [Header("UI")]
    public GameObject pressEMessage;   
    public Image fadeImage;           

    [Header("Escenarios")]
    public GameObject escenarioActual;  
    public GameObject bosque;           

    private bool playerCerca = false;

    void Start()
    {
        if (pressEMessage != null) pressEMessage.SetActive(false);
        if (fadeImage != null) fadeImage.gameObject.SetActive(false);

        if (bosque != null) bosque.SetActive(false);
    }

    void Update()
    {
        if (playerCerca && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(FadeAndSwapScene());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCerca = true;
            if (pressEMessage != null) pressEMessage.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCerca = false;
            if (pressEMessage != null) pressEMessage.SetActive(false);
        }
    }

    IEnumerator FadeAndSwapScene()
    {
        fadeImage.gameObject.SetActive(true);

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (escenarioActual != null)
            escenarioActual.SetActive(false);

        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemigo in enemigos)
        {
            Destroy(enemigo);
        }

        if (bosque != null)
            bosque.SetActive(true);

        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
