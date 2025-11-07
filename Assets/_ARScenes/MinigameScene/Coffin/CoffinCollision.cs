using UnityEngine;

public class CoffinCollision : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] string playerTag = "Car";
    
    private bool hasTriggeredGameOver = false;

    void OnTriggerEnter(Collider other)
    {
        // Verificar si el ataúd toca el coche
        if (hasTriggeredGameOver) return;
        
        if (other.CompareTag(playerTag) || other.name == "Car" || other.GetComponent<CarController>() != null)
        {
            hasTriggeredGameOver = true;
            TriggerGameOver();
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // También detectar colisiones físicas
        if (hasTriggeredGameOver) return;
        
        GameObject other = collision.gameObject;
        if (other.CompareTag(playerTag) || other.name == "Car" || other.GetComponent<CarController>() != null)
        {
            hasTriggeredGameOver = true;
            TriggerGameOver();
        }
    }
    
    private void TriggerGameOver()
    {
        GameOverManager gameOverManager = GameOverManager.Instance;
        if (gameOverManager != null)
        {
            gameOverManager.GameOver();
        }
        else
        {
            Debug.LogError("GameOverManager not found! Cannot trigger game over.");
        }
    }
}

