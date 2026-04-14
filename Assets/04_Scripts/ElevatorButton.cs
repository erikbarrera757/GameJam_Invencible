using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Para Keyboard.current

public class ElevatorButton : MonoBehaviour
{
    [Header("UI")]
    public GameObject pressEMessage;   // Texto UI en pantalla (Canvas Overlay)
    public Image fadeImage;            // Imagen negra para el fade

    [Header("Laboratorios")]
    public GameObject laboratorioViejo;   // Primer laboratorio (activo al inicio)
    public GameObject laboratorioNuevo;   // Nuevo laboratorio (deshabilitado al inicio)

    private bool playerCerca = false;

    void Start()
    {
        if (pressEMessage != null) pressEMessage.SetActive(false);
        if (fadeImage != null) fadeImage.gameObject.SetActive(false);

        // Aseguramos que el nuevo laboratorio esté deshabilitado al inicio
        if (laboratorioNuevo != null) laboratorioNuevo.SetActive(false);
    }

    void Update()
    {
        if (playerCerca && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(FadeAndSwapLabs());
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

    IEnumerator FadeAndSwapLabs()
    {
        fadeImage.gameObject.SetActive(true);

        // Fade Out
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        // Mantener pantalla oscura un momento
        yield return new WaitForSeconds(1f);

        // Desactivar laboratorio viejo
        if (laboratorioViejo != null)
            laboratorioViejo.SetActive(false);

        // ?? Destruir todos los enemigos activos del laboratorio viejo
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemigo in enemigos)
        {
            Destroy(enemigo);
        }

        // Activar laboratorio nuevo
        if (laboratorioNuevo != null)
            laboratorioNuevo.SetActive(true);

        // Fade In
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
