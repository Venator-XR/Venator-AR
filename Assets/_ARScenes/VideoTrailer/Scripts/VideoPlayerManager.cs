using UnityEngine;
using UnityEngine.Video;
using System;

/// <summary>
/// Central manager for VideoPlayer control following Single Responsibility Principle.
/// Handles playback, volume, timeline control, and state management.
/// Uses events for decoupled communication with UI elements.
/// </summary>
public class VideoPlayerManager : MonoBehaviour
{
    [Header("Video Player Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    
    [Header("Audio Settings")]
    [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.5f;
    
    // Events for UI updates (Observer Pattern)
    public event Action<bool> OnPlayStateChanged;
    public event Action<float> OnVolumeChanged;
    public event Action<float> OnTimeChanged; // normalized time (0-1)
    public event Action<float> OnDurationChanged; // in seconds
    public event Action OnVideoReady;
    
    // Properties for external access
    public bool IsPlaying => videoPlayer != null && videoPlayer.isPlaying;
    public bool IsPaused => videoPlayer != null && videoPlayer.isPaused;
    public bool IsReady { get; private set; }
    public float Duration => IsReady ? (float)videoPlayer.length : 0f;
    public float CurrentTime => IsReady ? (float)videoPlayer.time : 0f;
    public float NormalizedTime => Duration > 0 ? CurrentTime / Duration : 0f;
    public float Volume
    {
        get => videoPlayer != null && videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct 
            ? videoPlayer.GetDirectAudioVolume(0) 
            : 0f;
        set => SetVolume(value);
    }
    
    private bool isInitialized = false;
    
    #region Initialization
    
    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayerManager: No VideoPlayer component found!");
            return;
        }
        
        InitializeVideoPlayer();
    }
    
    private void InitializeVideoPlayer()
    {
        // Setup audio output
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetDirectAudioVolume(0, defaultVolume);
        
        // Subscribe to VideoPlayer events
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.started += OnVideoStarted;
        videoPlayer.loopPointReached += OnVideoEnded;
        
        // Prepare the video
        videoPlayer.Prepare();
        
        isInitialized = true;
    }
    
    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.started -= OnVideoStarted;
            videoPlayer.loopPointReached -= OnVideoEnded;
        }
    }
    
    #endregion
    
    #region VideoPlayer Callbacks
    
    private void OnVideoPrepared(VideoPlayer source)
    {
        IsReady = true;
        Debug.Log($"Video prepared. Duration: {Duration:F2}s");
        OnVideoReady?.Invoke();
        OnDurationChanged?.Invoke(Duration);
    }
    
    private void OnVideoStarted(VideoPlayer source)
    {
        OnPlayStateChanged?.Invoke(true);
    }
    
    private void OnVideoEnded(VideoPlayer source)
    {
        OnPlayStateChanged?.Invoke(false);
    }
    
    #endregion
    
    #region Playback Control
    
    /// <summary>
    /// Play the video
    /// </summary>
    public void Play()
    {
        if (!IsReady)
        {
            Debug.LogWarning("Video not ready yet!");
            return;
        }
        
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            OnPlayStateChanged?.Invoke(true);
        }
    }
    
    /// <summary>
    /// Pause the video
    /// </summary>
    public void Pause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            OnPlayStateChanged?.Invoke(false);
        }
    }
    
    /// <summary>
    /// Toggle between play and pause
    /// </summary>
    public void TogglePlayPause()
    {
        if (IsPlaying)
            Pause();
        else
            Play();
    }
    
    /// <summary>
    /// Stop the video and reset to beginning
    /// </summary>
    public void Stop()
    {
        videoPlayer.Stop();
        OnPlayStateChanged?.Invoke(false);
        OnTimeChanged?.Invoke(0f);
    }
    
    #endregion
    
    #region Timeline Control
    
    /// <summary>
    /// Set video time in seconds
    /// </summary>
    /// <param name="timeInSeconds">Time in seconds</param>
    public void SetTime(float timeInSeconds)
    {
        if (!IsReady) return;
        
        timeInSeconds = Mathf.Clamp(timeInSeconds, 0f, Duration);
        videoPlayer.time = timeInSeconds;
        OnTimeChanged?.Invoke(NormalizedTime);
    }
    
    /// <summary>
    /// Set video time using normalized value (0-1)
    /// </summary>
    /// <param name="normalizedTime">Normalized time between 0 and 1</param>
    public void SetNormalizedTime(float normalizedTime)
    {
        if (!IsReady) return;
        
        normalizedTime = Mathf.Clamp01(normalizedTime);
        float targetTime = normalizedTime * Duration;
        videoPlayer.time = targetTime;
        OnTimeChanged?.Invoke(normalizedTime);
    }
    
    #endregion
    
    #region Volume Control
    
    /// <summary>
    /// Set volume (0-1)
    /// </summary>
    public void SetVolume(float volume)
    {
        if (!isInitialized || videoPlayer == null) return;
        
        volume = Mathf.Clamp01(volume);
        
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
            videoPlayer.SetDirectAudioVolume(0, volume);
            OnVolumeChanged?.Invoke(volume);
        }
    }
    
    /// <summary>
    /// Mute/unmute the video
    /// </summary>
    public void ToggleMute()
    {
        if (Volume > 0)
        {
            SetVolume(0);
        }
        else
        {
            SetVolume(defaultVolume);
        }
    }
    
    #endregion
    
    #region Update Loop
    
    private void Update()
    {
        // Continuously update time for UI elements
        if (IsReady && IsPlaying)
        {
            OnTimeChanged?.Invoke(NormalizedTime);
        }
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Get formatted time string (MM:SS)
    /// </summary>
    public string GetFormattedCurrentTime()
    {
        return FormatTime(CurrentTime);
    }
    
    /// <summary>
    /// Get formatted duration string (MM:SS)
    /// </summary>
    public string GetFormattedDuration()
    {
        return FormatTime(Duration);
    }
    
    /// <summary>
    /// Format seconds to MM:SS
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    
    #endregion
}
