using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinigameSpawner : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] GameObject coffinPrefab;
    [SerializeField] GameObject vampirePrefab;

    [Header("Spawn Settings")]
    [SerializeField] Transform spawnPosition;
    [SerializeField] float coffinSpawnInterval = 3f;
    [SerializeField] float vampireSpawnDelay = 2.5f;
    [SerializeField] float coffinLifetime = 5f;
    [SerializeField] bool autoDetectCarHeight = true;
    [SerializeField] float manualSpawnHeight = 0.2f;

    // 🔸 NUEVO: Tamaño del rectángulo de spawn (X = ancho, Y = largo)
    [SerializeField] Vector2 spawnRectangleSize = new Vector2(10f, 10f);

    private List<CoffinData> activeCoffins = new List<CoffinData>();
    private Coroutine spawnCoroutine;

    private class CoffinData
    {
        public GameObject coffin;
        public GameObject vampire;
        public float spawnTime;
    }

    void OnEnable()
    {
        CleanupAllObjects();
        spawnCoroutine = StartCoroutine(ContinuousSpawn());
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        CleanupAllObjects();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        CleanupExpiredCoffins();
    }

    private IEnumerator ContinuousSpawn()
    {
        while (true)
        {
            SpawnCoffinAndVampire();
            yield return new WaitForSeconds(coffinSpawnInterval);
        }
    }

    private void SpawnCoffinAndVampire()
    {   
        // 🔸 AHORA EL SPAWN ES RECTANGULAR
        float randomX = Random.Range(-spawnRectangleSize.x / 2f, spawnRectangleSize.x / 2f);
        float randomZ = Random.Range(-spawnRectangleSize.y / 2f, spawnRectangleSize.y / 2f);
        Vector3 finalSpawnPosition = new Vector3(
            spawnPosition.position.x + randomX,
            spawnPosition.position.y,
            spawnPosition.position.z + randomZ
        );

        if (coffinPrefab != null)
        {
            GameObject newCoffin = Instantiate(coffinPrefab, finalSpawnPosition, Quaternion.identity, transform);
            newCoffin.name = "Coffin_" + Time.time;

            CoffinCollision coffinCollision = newCoffin.GetComponent<CoffinCollision>();
            if (coffinCollision == null) coffinCollision = newCoffin.AddComponent<CoffinCollision>();

            CoffinData coffinData = new CoffinData
            {
                coffin = newCoffin,
                vampire = null,
                spawnTime = Time.time
            };

            activeCoffins.Add(coffinData);
            Debug.Log("Coffin spawned at position: " + finalSpawnPosition);

            StartCoroutine(SpawnVampireForCoffin(coffinData, finalSpawnPosition));
        }
        else
        {
            Debug.LogWarning("Coffin prefab not assigned in MinigameSpawner!");
        }
    }

    private IEnumerator SpawnVampireForCoffin(CoffinData coffinData, Vector3 spawnPos)
    {
        // Esperar el delay antes de spawnear el vampiro
        yield return new WaitForSeconds(vampireSpawnDelay);
        
        // Verificar que el ataúd aún existe
        if (coffinData.coffin == null || !activeCoffins.Contains(coffinData))
        {
            yield break;
        }
        
        // Spawn del vampiro dentro del ataúd
        if (vampirePrefab != null)
        {
            Vector3 vampirePosition = coffinData.coffin.transform.position;
            
            GameObject newVampire = Instantiate(vampirePrefab, vampirePosition, Quaternion.identity, transform);
            newVampire.name = "Vampire_" + Time.time;
            
            // Asegurar que el vampiro tenga el script de persecución
            VampireChase chaseScript = newVampire.GetComponent<VampireChase>();
            if (chaseScript == null)
            {
                chaseScript = newVampire.AddComponent<VampireChase>();
            }
            
            coffinData.vampire = newVampire;
            
            Debug.Log("Vampire spawned at position: " + vampirePosition);
        }
        else
        {
            Debug.LogWarning("Vampire prefab not assigned in MinigameSpawner!");
        }
    }

    private void CleanupExpiredCoffins()
    {
        for (int i = activeCoffins.Count - 1; i >= 0; i--)
        {
            CoffinData coffinData = activeCoffins[i];
            
            // Si el ataúd ha cumplido su tiempo de vida, destruirlo
            if (Time.time - coffinData.spawnTime >= coffinLifetime)
            {
                if (coffinData.coffin != null)
                {
                    Animator coffinAnimator = coffinData.coffin.GetComponentInChildren<Animator>();
                    ParticleSystem ps = coffinData.coffin.GetComponent<ParticleSystem>();
                    if (coffinAnimator != null)
                    {
                        coffinAnimator.SetTrigger("CloseCoffin");
                    }
                    if (ps != null)
                    {
                        // Reproducir las partículas 0.5 segundos después de activar la animación
                        IEnumerator PlayParticlesWithDelay()
                        {
                            yield return new WaitForSeconds(0.5f);
                            if (ps != null)
                            {
                                // Stop original so it doesn't play attached to the coffin
                                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                                // Clone the particle GameObject so it can live independently after the coffin is destroyed
                                GameObject psCloneGO = Instantiate(ps.gameObject, ps.transform.position, ps.transform.rotation);
                                psCloneGO.transform.SetParent(null);
                                ParticleSystem psClone = psCloneGO.GetComponent<ParticleSystem>();

                                if (psClone != null)
                                {
                                    var mainClone = psClone.main;
                                    mainClone.duration = 0.9f;
                                    psClone.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                                    // Match the coffin destroy delay used below (Destroy(coffinData.coffin, 1.5f))
                                    float destroyDelay = 1.5f;
                                    // We already waited 0.5s before reaching this point in the outer coroutine,
                                    // so wait the remaining time until the coffin is destroyed.
                                    float alreadyWaited = 0.5f;
                                    float remaining = Mathf.Max(0f, destroyDelay - alreadyWaited);

                                    yield return new WaitForSeconds(remaining);

                                    // Play the detached particle system after the coffin has been destroyed
                                    psClone.Play();

                                    // Compute an approximate lifetime to destroy the particle GameObject afterwards
                                    float startLifetime = 0.0f;
                                    var sl = mainClone.startLifetime;
                                    if (sl.mode == ParticleSystemCurveMode.Constant)
                                        startLifetime = sl.constant;
                                    else
                                        startLifetime = sl.constantMax; // fallback for curve modes

                                    float lifeTime = mainClone.duration + startLifetime;
                                    Destroy(psCloneGO, lifeTime + 0.1f);
                                }
                            }
                        }
                        StartCoroutine(PlayParticlesWithDelay());
                    }
                    Destroy(coffinData.coffin, 1.5f);
                }
                // El vampiro sigue vivo y persiguiendo
                activeCoffins.RemoveAt(i);
            }
        }
    }

    private void CleanupAllObjects()
    {
        // Destruir todos los ataúdes activos
        foreach (CoffinData coffinData in activeCoffins)
        {
            if (coffinData.coffin != null)
            {
                Destroy(coffinData.coffin);
            }
            // Los vampiros seguirán persiguiendo aunque se destruya el ataúd
        }
        
        activeCoffins.Clear();
    }
    
    public void CleanupAllOnGameOver()
    {
        // Detener el spawn
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        // Destruir todos los ataúdes y vampiros
        foreach (CoffinData coffinData in activeCoffins)
        {
            if (coffinData.coffin != null)
            {
                Destroy(coffinData.coffin);
            }
            if (coffinData.vampire != null)
            {
                Destroy(coffinData.vampire);
            }
        }
        
        activeCoffins.Clear();
    }
}

