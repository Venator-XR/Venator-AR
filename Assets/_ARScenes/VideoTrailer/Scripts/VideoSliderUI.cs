using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI slider for video timeline control using Unity's built-in Slider component.
/// Provides interactive scrubbing through the video timeline.
/// </summary>
[RequireComponent(typeof(Slider))]
public class VideoSliderUI : MonoBehaviour
{
    [Header("Manager Reference")]
    [SerializeField] private VideoPlayerManager videoManager;
    
    [Header("Settings")]
    [SerializeField] private bool updateWhileDragging = true;
    
    private Slider timelineSlider;
    private bool isDragging = false;
    
    private void Awake()
    {
        timelineSlider = GetComponent<Slider>();
        
        if (timelineSlider == null)
        {
            Debug.LogError("VideoSliderUI: No Slider component found!");
            return;
        }
        
        // Configure slider
        timelineSlider.minValue = 0f;
        timelineSlider.maxValue = 1f;
        timelineSlider.value = 0f;
        
        // Subscribe to slider events
        timelineSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    private void Start()
    {
        if (videoManager == null)
        {
            Debug.LogError("VideoSliderUI: No VideoPlayerManager assigned!");
            return;
        }
        
        // Subscribe to manager events
        videoManager.OnTimeChanged += UpdateSliderPosition;
        videoManager.OnVideoReady += OnVideoReady;
        
        // Initialize position
        if (videoManager.IsReady)
        {
            UpdateSliderPosition(videoManager.NormalizedTime);
        }
    }
    
    private void OnDestroy()
    {
        if (timelineSlider != null)
        {
            timelineSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        
        if (videoManager != null)
        {
            videoManager.OnTimeChanged -= UpdateSliderPosition;
            videoManager.OnVideoReady -= OnVideoReady;
        }
    }
    
    #region Event Handlers
    
    private void OnVideoReady()
    {
        UpdateSliderPosition(0f);
    }
    
    private void OnSliderValueChanged(float value)
    {
        if (videoManager != null && videoManager.IsReady)
        {
            // Only update video time when user is interacting with the slider
            if (isDragging || updateWhileDragging)
            {
                videoManager.SetNormalizedTime(value);
            }
        }
    }
    
    private void UpdateSliderPosition(float normalizedTime)
    {
        if (timelineSlider == null || isDragging) return;
        
        // Update slider without triggering the onValueChanged event
        timelineSlider.SetValueWithoutNotify(normalizedTime);
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
            videoManager.OnTimeChanged -= UpdateSliderPosition;
            videoManager.OnVideoReady -= OnVideoReady;
        }
        
        videoManager = manager;
        
        if (videoManager != null)
        {
            videoManager.OnTimeChanged += UpdateSliderPosition;
            videoManager.OnVideoReady += OnVideoReady;
        }
    }
    
    #endregion
}
