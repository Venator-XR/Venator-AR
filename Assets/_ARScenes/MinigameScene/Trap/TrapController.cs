using UnityEngine;
using System.Collections; // Required for Coroutines (IEnumerator)

[RequireComponent(typeof(Collider))]
public class TrapController : MonoBehaviour
{
    public Animator animator;
    private string activationTrigger = "TriggerTrap";

    private void Awake()
    {
        if (animator == null)
        {
            // It looks on this object AND all children.
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("TrapController could not find an Animator in its children!", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (animator == null) return; 

        if (other.CompareTag("Car"))
        {
            // --- This is the new logic ---
            // 1. We enable the animator immediately.
            if (animator != null)
            {
                animator.enabled = true; 
            }
            
            // 2. We START the coroutine, which contains the delay.
            StartCoroutine(DelayedTrigger());
        }
    }

    /// <summary>
    /// A coroutine that waits for 1 second then triggers the animation.
    /// </summary>
    private IEnumerator DelayedTrigger()
    {
        // 1. This line pauses the function for 1 second.
        yield return new WaitForSeconds(1f);

        // 2. This line runs AFTER the 1-second delay.
        animator.SetTrigger(activationTrigger);
    }
}