using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitButton();
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;              // por si venías de un pause/game over
        SceneManager.LoadScene("GameTP2");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuTP2");
    }

    public void ExitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}