// MenuButtons.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    // Cargar escena de juego
    public void LoadJuego()
    {
        Time.timeScale = 1f;              // por si venías de un pause/game over
        SceneManager.LoadScene("Juego");
    }

    // Cargar escena de menú principal
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Salir del juego
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // parar Play en el Editor
#elif UNITY_WEBGL
        // En WebGL no se puede cerrar la pestaña; podrías ocultar este botón o ir a una URL
        // Application.Quit() no hace nada en WebGL.
#else
        Application.Quit();
#endif
    }
}

