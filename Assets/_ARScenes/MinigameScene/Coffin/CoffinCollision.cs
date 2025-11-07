using System.Collections;
using UnityEngine;

public class CoffinCollision : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private string playerTag = "Car";
    [SerializeField] private float particleDuration = 0.5f;

    private bool hasTriggeredGameOver = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasTriggeredGameOver) return;
        if (!collision.gameObject.CompareTag(playerTag)) return;

        var carController = collision.gameObject.GetComponent<CarController>();
        if (carController == null) return;

        hasTriggeredGameOver = true;
        StartCoroutine(HandleCollision(carController, collision.gameObject));
    }

    private IEnumerator HandleCollision(CarController carController, GameObject carObject)
    {
        // Detener control del coche
        carController.enabled = false;

        // Intentar reproducir partículas si existen
        var particles = carObject.GetComponent<ParticleSystem>();
        if (particles != null)
        {
            particles.Play();
            yield return new WaitForSeconds(particleDuration);
        }

        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager not found! Cannot trigger game over.");
        }
    }
}
