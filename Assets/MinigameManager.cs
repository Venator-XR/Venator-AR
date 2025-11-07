using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private CarController carController;
    private MinigameSpawner minigameSpawner;
    private PlayableDirector playableDirector;

    [Header("UI Elements")]
    [SerializeField] private GameObject startCanvas;   // Canvas principal para empezar
    [SerializeField] private GameObject timeCounter;   // Su hijo tiene un Canvas

    private bool hasStartedCoroutine = false;

    private void Awake()
    {
        minigameSpawner = GetComponent<MinigameSpawner>();
        playableDirector = GetComponent<PlayableDirector>();

        if (!carController)
            Debug.LogError("CarController not assigned!");
        if (!startCanvas)
            Debug.LogError("StartCanvas not assigned!");
        if (!timeCounter)
            Debug.LogError("TimeCounter not assigned!");

        if (carController) carController.enabled = false;
        if (startCanvas) startCanvas.SetActive(false);
        if (timeCounter) timeCounter.SetActive(false);
    }

    private void Start()
    {
        // Si hay una timeline, iniciarla al entrar en escena
        if (playableDirector != null)
        {
            playableDirector.time = 0;
            playableDirector.Play();
        }
    }

    private void Update()
    {
        if (playableDirector == null || hasStartedCoroutine)
            return;

        // Cuando empieza la timeline, lanzamos el temporizador para mostrar el canvas de inicio
        if (playableDirector.state == PlayState.Playing && playableDirector.time < 0.05)
        {
            hasStartedCoroutine = true;
            StartCoroutine(WaitThenShowStartCanvas((float)playableDirector.playableAsset.duration));
        }
    }

    private IEnumerator WaitThenShowStartCanvas(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        startCanvas.SetActive(true);
        yield return StartCoroutine(FadeCanvas(startCanvas, 0f, 1f, 1f));
    }

    public void StartMinigame()
    {
        StartCoroutine(SwitchToGameplay());
    }

    private IEnumerator SwitchToGameplay()
    {
        // Fade out del canvas inicial
        yield return StartCoroutine(FadeCanvas(startCanvas, 1f, 0f, 0.5f));
        startCanvas.SetActive(false);

        // Activar contador y reiniciarlo
        timeCounter.SetActive(true);
        timeCounter.GetComponent<TimeCounter>().Restart();
        yield return StartCoroutine(FadeCanvas(timeCounter, 0f, 1f, 0.8f));

        // Activar gameplay
        carController.enabled = true;
        minigameSpawner.enabled = true;
    }

    // ---------------------------------------------------
    // RESET & REINICIO COMPLETO
    // ---------------------------------------------------

    public void ResetMinigameState()
    {
        Debug.LogWarning("RESET MINIGAME");
        StopAllCoroutines();

        hasStartedCoroutine = false;

        if (startCanvas != null)
            startCanvas.SetActive(false);

        if (timeCounter != null)
            timeCounter.SetActive(false);

        if (minigameSpawner != null)
        {
            minigameSpawner.enabled = false;
            minigameSpawner.CleanupAllOnGameOver();
        }

        if (carController != null)
            carController.enabled = false;

        if (playableDirector != null)
        {
            playableDirector.Stop();
            playableDirector.time = 0;
        }

        Debug.Log("[MinigameManager] Estado del minijuego reseteado completamente.");
    }

    public void RestartMinigameFlow()
    {
        ResetMinigameState();

        // Reiniciar timeline y mostrar canvas de nuevo
        if (playableDirector != null)
        {
            playableDirector.time = 0;
            playableDirector.Play();
        }

        StartCoroutine(ShowStartCanvasAfterTimeline());
    }

    private IEnumerator ShowStartCanvasAfterTimeline()
    {
        if (playableDirector == null)
            yield break;

        double duration = playableDirector.playableAsset.duration;
        yield return new WaitForSeconds((float)duration);

        if (startCanvas != null)
        {
            startCanvas.SetActive(true);
            yield return StartCoroutine(FadeCanvas(startCanvas, 0f, 1f, 1f));
        }
    }

    // ---------------------------------------------------
    // FADES DE UI
    // ---------------------------------------------------

    private IEnumerator FadeCanvas(GameObject target, float from, float to, float duration)
    {
        if (!target) yield break;

        Graphic[] graphics = target.GetComponentsInChildren<Graphic>(includeInactive: true);
        float t = 0f;
        SetGraphicsAlpha(graphics, from);

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            SetGraphicsAlpha(graphics, alpha);
            yield return null;
        }

        SetGraphicsAlpha(graphics, to);
    }

    private void SetGraphicsAlpha(Graphic[] graphics, float alpha)
    {
        foreach (Graphic g in graphics)
        {
            if (g == null) continue;
            Color c = g.color;
            c.a = alpha;
            g.color = c;
        }
    }
}
