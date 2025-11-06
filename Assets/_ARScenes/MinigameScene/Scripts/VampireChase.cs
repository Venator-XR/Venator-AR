using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VampireChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] float chaseSpeed = 2f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float minDistanceToPlayer = 1f;
    
    [Header("Player Reference")]
    [SerializeField] string playerTag = "Car";
    
    [Header("Rotation Fix")]
    [Tooltip("Offset de rotación para corregir la orientación del modelo. Si está tumbado 90º, prueba con (-90, 0, 0) o (90, 0, 0)")]
    [SerializeField] Vector3 rotationOffset = new Vector3(-90, 0, 0); // Por defecto -90 en X para corregir modelo tumbado
    
    private Transform playerTransform;
    private Rigidbody rb;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configurar Rigidbody para movimiento suave
        rb.freezeRotation = true;
        rb.useGravity = false;
        
        // Aplicar offset de rotación inicial si es necesario
        if (rotationOffset != Vector3.zero)
        {
            transform.rotation = Quaternion.Euler(rotationOffset);
        }
        
        FindPlayer();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null)
            {
                return; // No player found yet, wait
            }
        }
        
        if (!isChasing)
        {
            isChasing = true;
        }
        
        ChasePlayer();
    }

    private void FindPlayer()
    {
        // Primero intentar por tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("Vampire found player: " + player.name);
            return;
        }
        
        // Buscar el coche por nombre si no tiene tag
        GameObject car = GameObject.Find("Car");
        if (car != null)
        {
            playerTransform = car.transform;
            Debug.Log("Vampire found player by name: Car");
            return;
        }
        
        // Buscar cualquier objeto con CarController
        CarController carController = FindObjectOfType<CarController>();
        if (carController != null)
        {
            playerTransform = carController.transform;
            Debug.Log("Vampire found player via CarController");
            return;
        }
        
        // Si aún no se encuentra, buscar en DriveCarScene
        GameObject driveCarScene = GameObject.Find("DriveCarScene");
        if (driveCarScene != null)
        {
            Transform carTransform = driveCarScene.transform.Find("Car");
            if (carTransform != null)
            {
                playerTransform = carTransform;
                Debug.Log("Vampire found player in DriveCarScene");
            }
        }
    }

    private void ChasePlayer()
    {
        if (playerTransform == null) return;
        
        Vector3 directionToPlayer = (playerTransform.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;
        
        // Siempre mirar al jugador, incluso si está cerca
        if (directionToPlayer.magnitude > 0.01f)
        {
            // Calcular dirección en el plano horizontal para la rotación
            Vector3 horizontalDirection = directionToPlayer;
            horizontalDirection.y = 0f;
            
            if (horizontalDirection.magnitude > 0.01f)
            {
                Quaternion baseRotation = Quaternion.LookRotation(horizontalDirection.normalized);
                // Aplicar offset de rotación si existe (multiplicar para mantener la rotación base)
                Quaternion targetRotation = rotationOffset != Vector3.zero 
                    ? baseRotation * Quaternion.Euler(rotationOffset) 
                    : baseRotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        // Solo perseguir si está fuera del rango mínimo
        if (distanceToPlayer > minDistanceToPlayer)
        {
            // Mover hacia el jugador
            Vector3 moveDirection = directionToPlayer.normalized;
            Vector3 movement = moveDirection * chaseSpeed * Time.deltaTime;
            
            // Mantener la altura del jugador
            Vector3 newPosition = transform.position + movement;
            if (playerTransform != null)
            {
                newPosition.y = playerTransform.position.y;
            }
            
            // Usar Rigidbody para movimiento suave
            rb.MovePosition(newPosition);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}

