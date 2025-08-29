using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject portalPrefab;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform topLimit;
    public Transform bottomLimit;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public float timer = 60f;
    private int points = 0;
    private GameObject currentPortal;
    private bool isGameOver = false;
    private bool canRestart = false;

    void Start()
    {
        Time.timeScale = 1f;
        SpawnPortal();
        UpdateUI();
        gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGameOver)
        {
            // Input mientras está en game over
            if (canRestart)
            {
                if (Input.GetMouseButtonDown(0)) // click izquierdo
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SceneManager.LoadScene("MainMenuTP2");
                }
            }
            return;
        }

        // Timer
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        timerText.text = "TIME " + Mathf.Ceil(timer);

        if (timer <= 0 && !isGameOver)
        {
            EndGame();
        }
    }

    public void AddPoint()
    {
        points++;
        UpdateUI();
        SpawnPortal();
    }

    void SpawnPortal()
    {
        if (currentPortal != null)
        {
            Destroy(currentPortal);
        }

        float x = Random.Range(leftLimit.position.x, rightLimit.position.x);
        float y = Random.Range(bottomLimit.position.y, topLimit.position.y);
        Vector3 spawnPos = new Vector3(x, y, 0);

        currentPortal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);
        currentPortal.GetComponent<Portal>().Init(this);
    }

    void UpdateUI()
    {
        pointsText.text = "POINTS " + points;
    }

    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f; // pausa el juego

        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "GAME OVER\nPuntuación: " + points;

        StartCoroutine(EnableRestartAfterDelay(3f));
    }

    IEnumerator EnableRestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // usa tiempo real, no el de la escala
        canRestart = true;
        gameOverText.text += "\n\nClick Izquierdo = Reiniciar\nEscape = Menú";
    }
}