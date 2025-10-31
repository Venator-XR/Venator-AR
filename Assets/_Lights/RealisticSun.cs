using UnityEngine;

[RequireComponent(typeof(Light))]
public class RealisticSun : MonoBehaviour
{
    [Header("Configuración del Ciclo")]
    [Tooltip("Duración de un ciclo completo de 24h en segundos reales.")]
    public float dayDurationSeconds = 60f;

    [Tooltip("Hora de inicio (0 = medianoche, 0.25 = amanecer, 0.5 = mediodía, 0.75 = atardecer)")]
    [Range(0, 1)]
    public float startTimeOfDay = 0.25f; // Empezar al amanecer por defecto

    [Tooltip("La dirección 'Norte' del sol (rotación en Y). -30 = sol inclinado al sur.")]
    public float sunDirectionY = -30f;

    [Header("Propiedades de la Luz")]
    [Tooltip("Gradiente de color para el ciclo. (0 = medianoche, 0.25 = amanecer, 0.5 = mediodía)")]
    public Gradient sunColor;

    [Tooltip("Curva de intensidad. (0 = medianoche, 0.5 = mediodía)")]
    public AnimationCurve sunIntensity;

    private Light directionalLight;
    private float timeOfDay; // 0.0 a 1.0

    void Start()
    {
        directionalLight = GetComponent<Light>();
        if (directionalLight.type != LightType.Directional)
        {
            Debug.LogWarning("Este script está diseñado para una Luz Direccional.");
        }
        timeOfDay = startTimeOfDay;
    }

    void Update()
    {
        // 1. Avanzar el tiempo del día
        // Se incrementa basado en el tiempo real y la duración deseada del día
        timeOfDay += Time.deltaTime / dayDurationSeconds;
        timeOfDay %= 2.0f; // Asegura que el valor se mantenga entre 0 y 2

        // 2. Calcular la rotación del sol
        // Restamos 0.25 para que '0' sea el amanecer (rotación X = 0)
        // y '0.25' sea el mediodía (rotación X = 90)
        float sunAngleX = (timeOfDay - 0.25f) * 360f;

        // Aplicamos la rotación del arco (X) y la dirección fija (Y)
        transform.rotation = Quaternion.Euler(sunAngleX, sunDirectionY, 0);

        // 3. Aplicar color e intensidad desde las curvas
        float evaluationTime = timeOfDay;
        directionalLight.color = sunColor.Evaluate(evaluationTime);
        directionalLight.intensity = sunIntensity.Evaluate(evaluationTime);
    }
}