using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] UnityEngine.UI.Button acceptButton;
    
    [Header("Scene Manager")]
    [SerializeField] SceneManager sceneManager;
    
    private bool isGameOver = false;
    private static GameOverManager instance;
    
    public static GameOverManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameOverManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Asegurar que el canvas esté desactivado al inicio
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
    }
    
    void Start()
    {
        // Buscar SceneManager si no está asignado
        if (sceneManager == null)
        {
            sceneManager = FindObjectOfType<SceneManager>();
            if (sceneManager == null)
            {
                Debug.LogWarning("SceneManager not found! Game Over will not be able to return to menu.");
            }
        }
        
        // Configurar el botón de aceptar
        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        }
        else
        {
            Debug.LogWarning("Accept Button not assigned in GameOverManager!");
        }
    }
    
    public void GameOver()
    {
        if (isGameOver) return; // Evitar múltiples llamadas
        
        isGameOver = true;
        
        // Limpiar todos los vampiros y ataúdes
        CleanupAllEntities();
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        // Mostrar el canvas de Game Over
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game Over Canvas not assigned!");
        }
        
        Debug.Log("Game Over!");
    }
    
    private void CleanupAllEntities()
    {
        // Limpiar a través del MinigameSpawner si existe
        MinigameSpawner spawner = FindObjectOfType<MinigameSpawner>();
        if (spawner != null)
        {
            spawner.CleanupAllOnGameOver();
        }
        
        // También limpiar cualquier vampiro o ataúd que quede suelto
        // Buscar todos los vampiros
        VampireChase[] vampires = FindObjectsOfType<VampireChase>();
        foreach (VampireChase vampire in vampires)
        {
            if (vampire != null && vampire.gameObject != null)
            {
                Destroy(vampire.gameObject);
            }
        }
        
        // Buscar todos los ataúdes
        CoffinCollision[] coffins = FindObjectsOfType<CoffinCollision>();
        foreach (CoffinCollision coffin in coffins)
        {
            if (coffin != null && coffin.gameObject != null)
            {
                Destroy(coffin.gameObject);
            }
        }
        
        // Buscar por nombre también por si acaso (solo objetos activos en la escena)
        // Esto es una medida de seguridad adicional
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject rootObj in allObjects)
        {
            // Buscar recursivamente en hijos
            SearchAndDestroyEntities(rootObj.transform);
        }
    }
    
    private void SearchAndDestroyEntities(Transform parent)
    {
        // Verificar si este objeto es un ataúd o vampiro
        if (parent.name.StartsWith("Coffin_") || parent.name.StartsWith("Vampire_"))
        {
            Destroy(parent.gameObject);
            return;
        }
        
        // Buscar en hijos
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            SearchAndDestroyEntities(parent.GetChild(i));
        }
    }
    
    private void OnAcceptButtonClicked()
    {
        // Reanudar el tiempo antes de volver al menú
        Time.timeScale = 1f;
        
        // Volver al menú principal
        if (sceneManager != null)
        {
            sceneManager.BackToMenu();
        }
        else
        {
            Debug.LogError("SceneManager not found! Cannot return to menu.");
        }
        
        // Resetear el estado
        isGameOver = false;
        
        // Ocultar el canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
    }
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se reanude si el objeto se destruye
        Time.timeScale = 1f;
    }
}

