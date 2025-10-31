using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI button for play/pause control using Unity's built-in Button component.
/// Provides visual feedback through icon/sprite changes.
/// </summary>
[RequireComponent(typeof(Button))]
public class PlayPauseButtonUI : MonoBehaviour
{
    [Header("Manager Reference")]
    [SerializeField] private VideoPlayerManager videoManager;
    
    [Header("Visual Feedback")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;
    
    [Header("Color Feedback (Optional)")]
    [SerializeField] private bool useColorChange = false;
    [SerializeField] private Color playColor = Color.green;
    [SerializeField] private Color pauseColor = Color.red;
    
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("PlayPauseButtonUI: No Button component found!");
            return;
        }
        
        // Try to get image if not assigned
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }
        
        // Subscribe to button click
        button.onClick.AddListener(OnButtonClick);
    }
    
    private void Start()
    {
        if (videoManager == null)
        {
            Debug.LogError("PlayPauseButtonUI: No VideoPlayerManager assigned!");
            return;
        }
        
        // Subscribe to state changes
        videoManager.OnPlayStateChanged += UpdateVisualState;
        videoManager.OnVideoReady += OnVideoReady;
        
        // Initialize visual state
        if (videoManager.IsReady)
        {
            UpdateVisualState(videoManager.IsPlaying);
        }
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
        
        if (videoManager != null)
        {
            videoManager.OnPlayStateChanged -= UpdateVisualState;
            videoManager.OnVideoReady -= OnVideoReady;
        }
    }
    
    #region Event Handlers
    
    private void OnVideoReady()
    {
        UpdateVisualState(videoManager.IsPlaying);
    }
    
    private void OnButtonClick()
    {
        if (videoManager != null)
        {
            videoManager.TogglePlayPause();
        }
    }
    
    #endregion
    
    #region Visual Feedback
    
    /// <summary>
    /// Update visual state based on play/pause state
    /// </summary>
    private void UpdateVisualState(bool isPlaying)
    {
        if (buttonImage == null) return;
        
        // Change sprite
        if (isPlaying && pauseSprite != null)
        {
            buttonImage.sprite = pauseSprite;
        }
        else if (!isPlaying && playSprite != null)
        {
            buttonImage.sprite = playSprite;
        }
        
        // Optionally change color
        if (useColorChange)
        {
            buttonImage.color = isPlaying ? pauseColor : playColor;
        }
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
            videoManager.OnPlayStateChanged -= UpdateVisualState;
            videoManager.OnVideoReady -= OnVideoReady;
        }
        
        videoManager = manager;
        
        if (videoManager != null)
        {
            videoManager.OnPlayStateChanged += UpdateVisualState;
            videoManager.OnVideoReady += OnVideoReady;
            
            if (videoManager.IsReady)
            {
                UpdateVisualState(videoManager.IsPlaying);
            }
        }
    }
    
    #endregion
}
