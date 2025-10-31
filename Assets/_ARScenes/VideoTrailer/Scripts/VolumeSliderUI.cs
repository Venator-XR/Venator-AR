using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI slider for volume control using Unity's built-in Slider component.
/// Provides interactive volume adjustment with visual feedback.
/// </summary>
[RequireComponent(typeof(Slider))]
public class VolumeSliderUI : MonoBehaviour
{
    [Header("Manager Reference")]
    [SerializeField] private VideoPlayerManager videoManager;
    
    [Header("Volume Settings")]
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float maxVolume = 1f;
    
    [Header("Visual Feedback (Optional)")]
    [SerializeField] private Image fillImage;
    [SerializeField] private bool useDynamicFillColor = true;
    [SerializeField] private Color lowVolumeColor = Color.red;
    [SerializeField] private Color mediumVolumeColor = Color.yellow;
    [SerializeField] private Color highVolumeColor = Color.green;
    
    private Slider volumeSlider;
    private bool isDragging = false;
    
    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
        
        if (volumeSlider == null)
        {
            Debug.LogError("VolumeSliderUI: No Slider component found!");
            return;
        }
        
        // Configure slider
        volumeSlider.minValue = minVolume;
        volumeSlider.maxValue = maxVolume;
        volumeSlider.value = 0.5f;
        
        // Try to get fill image if not assigned
        if (fillImage == null && volumeSlider.fillRect != null)
        {
            fillImage = volumeSlider.fillRect.GetComponent<Image>();
        }
        
        // Subscribe to slider events
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    private void Start()
    {
        if (videoManager == null)
        {
            Debug.LogError("VolumeSliderUI: No VideoPlayerManager assigned!");
            return;
        }
        
        // Subscribe to volume changes
        videoManager.OnVolumeChanged += UpdateSliderFromVolume;
        
        // Initialize with current volume
        if (videoManager.IsReady)
        {
            UpdateSliderFromVolume(videoManager.Volume);
        }
        else
        {
            // Use default value
            UpdateSliderFromVolume(0.5f);
        }
    }
    
    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        
        if (videoManager != null)
        {
            videoManager.OnVolumeChanged -= UpdateSliderFromVolume;
        }
    }
    
    #region Event Handlers
    
    private void OnSliderValueChanged(float value)
    {
        if (videoManager != null)
        {
            videoManager.SetVolume(value);
        }
        
        // Update visual feedback
        if (useDynamicFillColor)
        {
            UpdateFillColor(value);
        }
    }
    
    private void UpdateSliderFromVolume(float volume)
    {
        if (volumeSlider == null || isDragging) return;
        
        // Update slider without triggering the onValueChanged event
        volumeSlider.SetValueWithoutNotify(volume);
        
        // Update visual feedback
        if (useDynamicFillColor)
        {
            UpdateFillColor(volume);
        }
    }
    
    #endregion
    
    #region Visual Feedback
    
    private void UpdateFillColor(float volume)
    {
        if (fillImage == null) return;
        
        float normalizedValue = Mathf.InverseLerp(minVolume, maxVolume, volume);
        Color fillColor;
        
        if (normalizedValue < 0.33f)
        {
            // Low volume - blend from low to medium
            fillColor = Color.Lerp(lowVolumeColor, mediumVolumeColor, normalizedValue / 0.33f);
        }
        else if (normalizedValue < 0.66f)
        {
            // Medium volume - blend from medium to high
            fillColor = Color.Lerp(mediumVolumeColor, highVolumeColor, (normalizedValue - 0.33f) / 0.33f);
        }
        else
        {
            // High volume
            fillColor = highVolumeColor;
        }
        
        fillImage.color = fillColor;
    }
    
    #endregion
    
    #region Drag Detection
    
    /// <summary>
    /// Call this method when user starts dragging (e.g., from EventTrigger)
    /// </summary>
    public void OnBeginDrag()
    {
        isDragging = true;
    }
    
    /// <summary>
    /// Call this method when user stops dragging (e.g., from EventTrigger)
    /// </summary>
    public void OnEndDrag()
    {
        isDragging = false;
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Set the video manager reference
    /// </summary>
    public void SetVideoManager(VideoPlayerManager manager)
    {
        if (videoManager != null)
        {
            videoManager.OnVolumeChanged -= UpdateSliderFromVolume;
        }
        
        videoManager = manager;
        
        if (videoManager != null)
        {
            videoManager.OnVolumeChanged += UpdateSliderFromVolume;
            UpdateSliderFromVolume(videoManager.Volume);
        }
    }
    
    #endregion
}
