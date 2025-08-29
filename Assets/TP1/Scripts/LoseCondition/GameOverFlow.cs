// GameOverFlow.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverFlow : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.onDeath.AddListener(ShowGameOver);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.onDeath.RemoveListener(ShowGameOver);
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void GoToMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
