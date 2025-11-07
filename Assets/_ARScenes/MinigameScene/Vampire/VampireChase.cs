using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VampireChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float minDistanceToPlayer = 1f;

    [Header("Player Reference")]
    [SerializeField] private string playerTag = "Car";

    [Header("Rotation Fix")]
    [Tooltip("Offset de rotación para corregir la orientación del modelo. Si está tumbado 90º, prueba (-90, 0, 0) o (90, 0, 0)")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(-90, 0, 0);

    private Transform player;
    private Rigidbody rb;
    private bool hasTriggeredGameOver;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (rotationOffset != Vector3.zero)
            transform.rotation = Quaternion.Euler(rotationOffset);

        LocatePlayer();
    }

    void Update()
    {
        if (Time.timeScale == 0f || hasTriggeredGameOver) return;

        if (player == null)
        {
            LocatePlayer();
            return;
        }

        Chase();
    }

    private void LocatePlayer()
    {
        // Orden de prioridad: Tag → CarController → Nombre → Escena padre
        if (GameObject.FindGameObjectWithTag(playerTag) is GameObject tagged)
        {
            player = tagged.transform;
        }
        else if (FindObjectOfType<CarController>() is CarController controller)
        {
            player = controller.transform;
        }
        else if (GameObject.Find("Car") is GameObject named)
        {
            player = named.transform;
        }
        else if (GameObject.Find("DriveCarScene")?.transform.Find("Car") is Transform sceneCar)
        {
            player = sceneCar;
        }

        if (player != null)
            Debug.Log($"[Vampire] Player found: {player.name}");
    }

    private void Chase()
    {
        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        if (distance < 0.001f) return;

        // Rotar hacia el jugador
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(flatDir.normalized);
            if (rotationOffset != Vector3.zero)
                lookRot *= Quaternion.Euler(rotationOffset);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        // Avanzar si no está demasiado cerca
        if (distance > minDistanceToPlayer)
        {
            Vector3 move = dir.normalized * chaseSpeed * Time.deltaTime;
            move.y = 0f; // Mantener plano
            rb.MovePosition(transform.position + move);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("TOCADO");
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
        Debug.LogWarning("HUNDIDO?");
        // Intentar reproducir partículas si existen
        var particles = carObject.GetComponent<ParticleSystem>();
        if (particles != null)
        {
            particles.Play();
            yield return new WaitForSeconds(0.5f);
        }

        TriggerGameOver();
    }

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag(playerTag) || obj.name == "Car" || obj.GetComponent<CarController>() != null;
    }

    private void TriggerGameOver()
    {
        hasTriggeredGameOver = true;
        GameOverManager.Instance?.GameOver();
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, player.position);
    }
}
