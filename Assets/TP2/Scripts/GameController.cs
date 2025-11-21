using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Prefabs de objetos")]
    public GameObject cementPrefab;
    public GameObject maderaPrefab;
    public GameObject ladrilloPrefab;
    public GameObject vidrioPrefab;

    [Header("Limites de spawn")]
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform topLimit;
    public Transform bottomLimit;

    [Header("UI")]
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;

    [Header("Gameplay")]
    public float timer = 60f;
    public float spawnInterval = 5f;

    [Header("Layout Manager")]
    public LayoutManager layoutManager; //  arrastrar el layout manager aquí en el inspector
    public DeliveryProgression deliveryProgression;

    private int points = 0;
    private bool isGameOver = false;
    private bool canRestart = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void Start()
    {
        Time.timeScale = 1f;
        UpdateUI();
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);

        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (isGameOver)
        {
            if (canRestart)
            {
                if (Input.GetMouseButtonDown(0))
                    SceneManager.LoadScene("MainMenuTP2");
            }
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        if (timerText != null) timerText.text = "TIME: " + Mathf.Ceil(timer);

        if (timer <= 0 && !isGameOver) EndGame();
    }

    IEnumerator SpawnRoutine()
    {
        while (!isGameOver)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObject()
    {
        if (deliveryProgression == null || !deliveryProgression.IsInitialized)
            return;

        GameObject prefab = deliveryProgression.GetRandomUnlockedMaterial(points);

        if (prefab == null)
            return;

        float x = Random.Range(leftLimit.position.x, rightLimit.position.x);
        float y = Random.Range(bottomLimit.position.y, topLimit.position.y);

        Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
    }

    public void AddPoints(int value, float timeBonus)
    {
        points += value;
        timer += timeBonus;
        UpdateUI();

        //  avisar al LayoutManager del nuevo puntaje
        if (layoutManager != null)
            layoutManager.OnScoreChanged(points);
    }

    public void SubtractPoints(int value, float timePenalty)
    {
        points -= value;
        timer -= timePenalty;
        if (timer < 0) timer = 0;
        UpdateUI();

        //  también avisar al LayoutManager por si baja de nivel
        if (layoutManager != null)
            layoutManager.OnScoreChanged(points);
    }

    void UpdateUI()
    {
        if (pointsText != null) pointsText.text = "POINTS: " + points;
    }

    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER\nPOINTS: " + points;
            StartCoroutine(EnableRestartAfterDelay(3f));
        }
    }

    IEnumerator EnableRestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        canRestart = true;
        if (gameOverText != null)
            gameOverText.text += "\n\nClutch = Restart";
    }
}
