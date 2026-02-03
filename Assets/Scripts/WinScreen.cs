using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Text finalMovesText;
    [SerializeField] private Text finalTimeText;
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
    }

    public void ShowWinScreen(int score, int moves, float time)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Score: {score}";

            if (finalMovesText != null)
                finalMovesText.text = $"Moves: {moves}";

            if (finalTimeText != null)
                finalTimeText.text = $"Time: {time:F2}s";

            Debug.Log("Win Screen Shown!");
        }
    }

    public void HideWinScreen()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void OnPlayAgainClicked()
    {
        HideWinScreen();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
    }
}
