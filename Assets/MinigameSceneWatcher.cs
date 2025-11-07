using UnityEngine;

public class MinigameSceneWatcher : MonoBehaviour
{
    MinigameManager minigameManager;

    void Awake()
    {
        minigameManager = FindObjectOfType<MinigameManager>();
    }

    void OnEnable()
    {
        // Cuando el objeto se activa en escena: reiniciamos el flujo (cinemática -> start canvas)
        if (minigameManager != null)
        {
            minigameManager.RestartMinigameFlow();
            Debug.Log("[Watcher] MinigameScene OnEnable -> RestartMinigameFlow()");
        }
    }

    void OnDisable()
    {
        // Cuando se desactiva: limpiamos estado y corutinas para evitar residuos al volver
        if (minigameManager != null)
        {
            minigameManager.ResetMinigameState();
            Debug.Log("[Watcher] MinigameScene OnDisable -> ResetMinigameState()");
        }

        // Asegurarse de que el tiempo se reanude (por si GameOver puso Time.timeScale = 0)
        Time.timeScale = 1f;
    }
}