using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime;
    private bool isPaused = true; // Start in paused state
    public int minutes;
    public int seconds;

    void Update()
    {
        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        minutes = Mathf.FloorToInt(elapsedTime / 60);
        seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void StartCounter()
    {
        isPaused = false;
        UpdateTimerText();
    }

    public void Restart()
    {
        elapsedTime = 0;
        UpdateTimerText();
    }
}

