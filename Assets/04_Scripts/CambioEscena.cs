using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public void IrAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void EmpezarJuego()
    {
        // Se asegura de cargar el nivel principal
        SceneManager.LoadScene("SampleScene");
    }

    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    // NUEVA FUNCIÓN: Para ganar el juego
    public void GanarJuego()
    {
        // Carga la escena que acabas de crear
        SceneManager.LoadScene("VictoryScene");
    }
}