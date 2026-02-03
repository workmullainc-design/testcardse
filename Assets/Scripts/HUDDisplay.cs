using UnityEngine;
using UnityEngine.UI;

public class HUDDisplay : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text movesText;
    [SerializeField] private Text timerText;

    private void Start()
    {
        // Auto-find Text components if not assigned
        if (scoreText == null || movesText == null || timerText == null)
        {
            AutoFindTextComponents();
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {GameManager.Instance.Score}";
        }

        if (movesText != null)
        {
            movesText.text = $"Moves: {GameManager.Instance.Moves}";
        }

        if (timerText != null)
        {
            timerText.text = $"Time: {GameManager.Instance.ElapsedTime:F1}s";
        }
    }

    private void AutoFindTextComponents()
    {
        Text[] allTexts = FindObjectsOfType<Text>();
        foreach (Text text in allTexts)
        {
            string name = text.gameObject.name.ToLower();
            if (name.Contains("score") && scoreText == null)
                scoreText = text;
            else if (name.Contains("move") && movesText == null)
                movesText = text;
            else if (name.Contains("time") && timerText == null)
                timerText = text;
        }

        if (scoreText == null)
            Debug.LogWarning("HUDDisplay: Could not find ScoreText");
        if (movesText == null)
            Debug.LogWarning("HUDDisplay: Could not find MovesText");
        if (timerText == null)
            Debug.LogWarning("HUDDisplay: Could not find TimerText");
    }
}
