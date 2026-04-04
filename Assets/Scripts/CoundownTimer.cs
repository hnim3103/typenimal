using TMPro;
using UnityEngine;

public class CoundownTimer : MonoBehaviour
{
    public TypeManager typeManager;
    public TMP_Text timerDisplay;

    public int averageWPM = 40;

    private bool timerRunning = false;
    private float timeRemaining = 60f;
    private string targetText;

    void Update()
    {
        if (!timerRunning)
            return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining != 0)
                UpdateDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            timerRunning = false;
            UpdateDisplay(0);
            TimerEnded();
        }
    }

    public void StartTimer(string text)
    {
        targetText = text;
        timeRemaining = CalculateStartTime();
        timerRunning = true;

    }

    void UpdateDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimerEnded()
    {
        Debug.Log("Time's up!");
        timerRunning = false;
        typeManager.StopGame();
        typeManager.inputField.interactable = false;
    }

    float CalculateStartTime()
    {
        if (string.IsNullOrEmpty(targetText))
            return 60f;

        float charCount = targetText.Length;
        float words = charCount / 5f;

        return (words / averageWPM) * 60f;
    }
}
