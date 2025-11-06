using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinigameSpawner : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] GameObject coffinPrefab;
    [SerializeField] GameObject vampirePrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] Vector3 spawnPosition = Vector3.zero;
    [SerializeField] float coffinSpawnInterval = 5f; // Cada cuántos segundos aparece un ataúd
    [SerializeField] float vampireSpawnDelay = 1f; // Tiempo después del ataúd para spawnear vampiro
    [SerializeField] float coffinLifetime = 10f; // Tiempo antes de que desaparezca el ataúd
    [SerializeField] bool autoDetectCarHeight = true;
    [SerializeField] float manualSpawnHeight = 0.2f;
    [SerializeField] float spawnRadius = 5f; // Radio aleatorio para spawnear ataúdes
    
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
        // Limpiar objetos anteriores
        CleanupAllObjects();
        
        // Iniciar el spawn continuo
        spawnCoroutine = StartCoroutine(ContinuousSpawn());
    }

    void OnDisable()
    {
        // Detener el spawn
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        CleanupAllObjects();
    }

    void Update()
    {
        // No hacer nada si el juego está pausado (game over)
        if (Time.timeScale == 0f) return;
        
        // Limpiar ataúdes que han cumplido su tiempo de vida
        CleanupExpiredCoffins();
    }

    private IEnumerator ContinuousSpawn()
    {
        while (true)
        {
            // Spawnear un nuevo ataúd
            SpawnCoffinAndVampire();
            
            // Esperar el intervalo antes del siguiente spawn
            yield return new WaitForSeconds(coffinSpawnInterval);
        }
    }

    private void SpawnCoffinAndVampire()
    {
        // Obtener la altura del Car para spawnear a la misma altura
        float spawnHeight = GetCarHeight();
        
        // Posición aleatoria alrededor del punto de spawn
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 finalSpawnPosition = new Vector3(
            spawnPosition.x + randomCircle.x, 
            spawnHeight, 
            spawnPosition.z + randomCircle.y
        );
        
            // Spawn del ataúd
        if (coffinPrefab != null)
        {
            GameObject newCoffin = Instantiate(coffinPrefab, finalSpawnPosition, Quaternion.identity, transform);
            newCoffin.name = "Coffin_" + Time.time;
            
            // Asegurar que el ataúd tenga el script de colisión
            CoffinCollision coffinCollision = newCoffin.GetComponent<CoffinCollision>();
            if (coffinCollision == null)
            {
                coffinCollision = newCoffin.AddComponent<CoffinCollision>();
            }
            
            CoffinData coffinData = new CoffinData
            {
                coffin = newCoffin,
                vampire = null,
                spawnTime = Time.time
            };
            
            activeCoffins.Add(coffinData);
            
            Debug.Log("Coffin spawned at position: " + finalSpawnPosition + " (Car height: " + spawnHeight + ")");
            
            // Spawnear vampiro después del delay
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
                    Destroy(coffinData.coffin);
                }
                // El vampiro sigue vivo y persiguiendo
                activeCoffins.RemoveAt(i);
            }
        }
    }

    private float GetCarHeight()
    {
        if (!autoDetectCarHeight)
        {
            return manualSpawnHeight;
        }
        
        // Buscar el Car por tag
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        if (car == null)
        {
            // Buscar por nombre
            car = GameObject.Find("Car");
        }
        
        if (car != null)
        {
            float carHeight = car.transform.position.y;
            return carHeight;
        }
        
        // Buscar CarController
        CarController carController = FindObjectOfType<CarController>();
        if (carController != null)
        {
            float carHeight = carController.transform.position.y;
            return carHeight;
        }
        
        // Buscar en DriveCarScene
        GameObject driveCarScene = GameObject.Find("DriveCarScene");
        if (driveCarScene != null)
        {
            Transform carTransform = driveCarScene.transform.Find("Car");
            if (carTransform != null)
            {
                float carHeight = carTransform.position.y;
                return carHeight;
            }
        }
        
        // Si no se encuentra, usar altura manual
        return manualSpawnHeight;
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

