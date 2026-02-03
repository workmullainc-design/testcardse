using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text movesText;
    [SerializeField] private Text timerText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button playAgainButton;

    // Difficulty/Layout selection buttons
    [SerializeField] private Button size2x2Button;
    [SerializeField] private Button size3x3Button;
    [SerializeField] private Button size4x4Button;
    [SerializeField] private Button size5x6Button;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverScoreText;

    private void Start()
    {
        // Setup button listeners
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);
        
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        
        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadButtonClicked);
        
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);

        // Setup size selection buttons
        if (size2x2Button != null)
            size2x2Button.onClick.AddListener(() => SelectGameSize(2, 2));
        
        if (size3x3Button != null)
            size3x3Button.onClick.AddListener(() => SelectGameSize(3, 3));
        
        if (size4x4Button != null)
            size4x4Button.onClick.AddListener(() => SelectGameSize(4, 4));
        
        if (size5x6Button != null)
            size5x6Button.onClick.AddListener(() => SelectGameSize(5, 6));

        // Hide game over panel initially
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void UpdateScoreDisplay(int score, int moves, float time)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        
        if (movesText != null)
            movesText.text = $"Moves: {moves}";
        
        if (timerText != null)
            timerText.text = $"Time: {time:F1}s";
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverScoreText != null)
                gameOverScoreText.text = $"Final Score: {GameManager.Instance.Score}\n" +
                                        $"Moves: {GameManager.Instance.Moves}\n" +
                                        $"Time: {GameManager.Instance.ElapsedTime:F2}s";
        }
    }

    private void OnResetButtonClicked()
    {
        GameManager.Instance.ResetGame();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnSaveButtonClicked()
    {
        var gameManager = GameManager.Instance;
        SaveLoadManager.Instance.SaveGame(
            gameManager.Score, 
            gameManager.Moves, 
            gameManager.Rows, 
            gameManager.Columns, 
            gameManager.ElapsedTime,
            gameManager.GetMatchedCardIds()
        );
        Debug.Log("Game saved!");
    }

    private void OnLoadButtonClicked()
    {
        if (SaveLoadManager.Instance.TryLoadGame(out GameSaveData data))
        {
            GameManager.Instance.SetGridSize(data.rows, data.columns);
            Debug.Log("Game loaded!");
        }
        else
        {
            Debug.Log("No save file to load.");
        }
    }

    private void OnPlayAgainClicked()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        GameManager.Instance.ResetGame();
    }

    private void SelectGameSize(int rows, int columns)
    {
        GameManager.Instance.SetGridSize(rows, columns);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
