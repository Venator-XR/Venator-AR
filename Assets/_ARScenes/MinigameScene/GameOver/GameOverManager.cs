using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI finalTimeTxt;
    [SerializeField] private GameObject mainMenuBtn;

    [Header("Scene Objects")]
    [SerializeField] private GameObject mainMenuScene;     // tu “escena” de menú
    [SerializeField] private GameObject minigameScene;     // tu “escena” del minijuego

    private bool isGameOver = false;
    private static GameOverManager instance;

    public static GameOverManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameOverManager>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Pausar el tiempo de juego
        Time.timeScale = 0f;

        // Limpiar vampiros, ataúdes, etc.
        CleanupAllEntities();

        // Mostrar el canvas
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        // Mostrar el tiempo final
        var tc = FindAnyObjectByType<TimeCounter>();
        if (tc != null && finalTimeTxt != null)
        {
            int minutes = Mathf.FloorToInt(tc.minutes);
            int seconds = Mathf.FloorToInt(tc.seconds);
            finalTimeTxt.text = $"Has sobrevivido {minutes:00}:{seconds:00}";
        }

        // Parar minijuego por completo
        var minigameManager = FindAnyObjectByType<MinigameManager>();
        if (minigameManager != null)
            minigameManager.ResetMinigameState();

        if (mainMenuBtn != null)
            mainMenuBtn.SetActive(true);

        Debug.Log("[GameOverManager] GAME OVER activado");
    }

    private void CleanupAllEntities()
    {
        MinigameSpawner spawner = FindAnyObjectByType<MinigameSpawner>();
        if (spawner != null)
            spawner.CleanupAllOnGameOver();

        foreach (var v in FindObjectsOfType<VampireChase>())
            if (v != null) Destroy(v.gameObject);

        foreach (var c in FindObjectsOfType<CoffinCollision>())
            if (c != null) Destroy(c.gameObject);
    }

    // -----------------------
    // BOTÓN REINTENTAR
    // -----------------------
    public void OnRetryButtonClicked()
    {
        Time.timeScale = 1f;

        var minigameManager = FindAnyObjectByType<MinigameManager>();
        if (minigameManager != null)
        {
            // reset completo y volver a flujo inicial
            minigameManager.ResetMinigameState();
            minigameManager.RestartMinigameFlow();
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        isGameOver = false;

        Debug.Log("[GameOverManager] Reinicio del minijuego ejecutado");
    }

    // -----------------------
    // BOTÓN MENÚ PRINCIPAL
    // -----------------------
    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;

        var minigameManager = FindAnyObjectByType<MinigameManager>();
        if (minigameManager != null)
            minigameManager.ResetMinigameState();

        if (minigameScene != null)
            minigameScene.SetActive(false);

        if (mainMenuScene != null)
            mainMenuScene.SetActive(true);

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        isGameOver = false;

        Debug.Log("[GameOverManager] Vuelta al menú principal");
    }

    void OnDisable()
    {
        // Asegurar que no se quede pausado si se apaga el objeto
        Time.timeScale = 1f;
        isGameOver = false;
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);
    }
}
