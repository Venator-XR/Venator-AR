using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TrapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform carTransform;

    [Header("Settings")]
    [SerializeField] private string activationTrigger = "TriggerTrap";
    [SerializeField] private bool followCarHeight = true;

    [Header("Trap Activation")]
    [SerializeField] private string vampireTag = "Vampire";
    [SerializeField] private float triggerDelay = 1f;
    [SerializeField] private float activationDelay = 0f;
    [SerializeField] private float activeDuration = 2f;

    private bool trapActive;
    private Coroutine activationRoutine;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("TrapController could not find an Animator in its children!", this);

        if (carTransform == null)
        {
            GameObject car = GameObject.FindGameObjectWithTag("Car");
            if (car != null) carTransform = car.transform;
        }
    }

    private void Update()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            Vector3 pos = transform.position;
            pos.x = parent.position.x;
            pos.z = parent.position.z;
            transform.position = pos;
        }

        if (followCarHeight && carTransform != null)
        {
            // Mantiene la trampa a la altura del coche (mundo)
            Vector3 pos = transform.position;
            pos.y = carTransform.position.y + 0.05f;
            transform.position = pos;

            // Opcional: corregir rotación si el target rota o tiembla
            transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (animator != null && activationRoutine == null)
            {
                animator.enabled = true;
                activationRoutine = StartCoroutine(TriggerTrapSequence());
            }
            return;
        }

        TryHandleVampire(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHandleVampire(other);
    }

    private void TryHandleVampire(Collider other)
    {
        if (!trapActive) return;
        if (string.IsNullOrEmpty(vampireTag)) return;
        if (!other.CompareTag(vampireTag)) return;

        Destroy(other.gameObject);
    }

    private IEnumerator TriggerTrapSequence()
    {
        trapActive = false;

        if (triggerDelay > 0f)
            yield return new WaitForSeconds(triggerDelay);

        if (animator != null)
            animator.SetTrigger(activationTrigger);

        if (activationDelay > 0f)
            yield return new WaitForSeconds(activationDelay);

        trapActive = true;
        yield return new WaitForSeconds(activeDuration);
        trapActive = false;

        activationRoutine = null;
    }
}