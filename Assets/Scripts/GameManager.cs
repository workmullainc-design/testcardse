using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    
    private int score = 0;
    private int moves = 0;
    private float elapsedTime = 0f;
    private bool gameActive = false;
    private List<int> matchedCardIds = new List<int>();
    private List<Card> currentFlippedCards = new List<Card>();
    
    // UI References
    private Text scoreText;
    private Text movesText;
    private Text timerText;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    public int Score => score;
    public int Moves => moves;
    public float ElapsedTime => elapsedTime;
    public int Rows => rows;
    public int Columns => columns;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();

        FindUIReferences();
        InitializeNewGame();
    }

    private void Update()
    {
        if (gameActive)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void InitializeNewGame()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        score = 0;
        moves = 0;
        elapsedTime = 0f;
        gameActive = true;
        matchedCardIds.Clear();
        currentFlippedCards.Clear();

        // Initialize grid with rows and columns
        gridManager.InitializeGrid(rows, columns);
        
        // Setup cards with sprites
        SetupCards();
        
        UpdateScoreUI();
    }

    public void SetupCards()
    {
        Card[] cards = gridManager.Cards;
        int totalCards = gridManager.GetTotalCards();
        int pairsCount = totalCards / 2;

        if (cardSprites == null || cardSprites.Length == 0)
        {
            Debug.LogError("GameManager: CardSprites array is empty!");
            return;
        }

        Debug.Log($"GameManager: Setting up {totalCards} cards with {cardSprites.Length} sprite types");

        // Create a list of card IDs (pairs)
        List<int> cardIds = new List<int>();
        for (int i = 0; i < pairsCount; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        // Shuffle the card IDs
        for (int i = cardIds.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = cardIds[i];
            cardIds[i] = cardIds[randomIndex];
            cardIds[randomIndex] = temp;
        }

        // Assign to cards
        for (int i = 0; i < cards.Length; i++)
        {
            // Use cardIds[i] directly, but cap it to available sprites
            int pairId = cardIds[i];
            int spriteIndex = pairId % cardSprites.Length;
            Sprite sprite = cardSprites[spriteIndex];
            
            if (sprite == null)
            {
                Debug.LogError($"GameManager: Sprite at index {spriteIndex} is NULL!");
            }
            else
            {
                Debug.Log($"GameManager: Assigning card {i} ID={pairId}, sprite={sprite.name}");
            }
            
            cards[i].Initialize(pairId, sprite);
        }
    }

    public void OnCardFlipped(Card card)
    {
        if (!gameActive || currentFlippedCards.Contains(card))
            return;

        card.Flip();
        currentFlippedCards.Add(card);

        // Allow continuous flipping without waiting
        if (currentFlippedCards.Count >= 2)
        {
            moves++;
            UpdateScoreUI();
            
            // Check if the two cards match
            Card card1 = currentFlippedCards[0];
            Card card2 = currentFlippedCards[1];

            if (card1.CardId == card2.CardId)
            {
                // Cards match!
                score += 10;
                matchedCardIds.Add(card1.CardId);
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMatch();
                }
                
                // Fade out matched cards
                StartCoroutine(MarkCardsAsMatched(card1, card2));
            }
            else
            {
                // Cards don't match, flip them back after delay
                StartCoroutine(FlipCardsBackAfterDelay(card1, card2));
            }

            currentFlippedCards.Clear();
        }

        UpdateScoreUI();
    }

    private IEnumerator MarkCardsAsMatched(Card card1, Card card2)
    {
        yield return new WaitForSeconds(0.3f);
        
        card1.SetMatched();
        card2.SetMatched();

        // Check for win condition
        if (matchedCardIds.Count * 2 == gridManager.GetTotalCards())
        {
            WinGame();
        }
    }

    private IEnumerator FlipCardsBackAfterDelay(Card card1, Card card2)
    {
        yield return new WaitForSeconds(1f);
        
        card1.ResetFlip();
        card2.ResetFlip();
    }

    private void WinGame()
    {
        gameActive = false;
        
        WinScreen winScreen = FindObjectOfType<WinScreen>();
        if (winScreen != null)
        {
            winScreen.ShowWinScreen(score, moves, elapsedTime);
        }
        Debug.Log($"Game Won! Score: {score}, Moves: {moves}, Time: {elapsedTime:F2}s");
        
        // Save the game
        SaveLoadManager.Instance.SaveGame(score, moves, rows, columns, elapsedTime, matchedCardIds);
    }

    private void FindUIReferences()
    {
        // Find UI text elements - adjust paths as needed for your UI setup
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Text[] allTexts = canvas.GetComponentsInChildren<Text>();
            foreach (Text text in allTexts)
            {
                if (text.name == "ScoreText")
                    scoreText = text;
                else if (text.name == "MovesText")
                    movesText = text;
                else if (text.name == "TimerText")
                    timerText = text;
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        
        if (movesText != null)
            movesText.text = $"Moves: {moves}";
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"Time: {elapsedTime:F1}s";
    }

    public void SetGridSize(int newRows, int newColumns)
    {
        int totalCards = newRows * newColumns;
        if (totalCards % 2 != 0)
        {
            Debug.LogWarning($"Grid size {newRows}x{newColumns} has odd number of cards. Please use even total cards.");
            return;
        }

        int pairsNeeded = totalCards / 2;
        if (cardSprites == null || cardSprites.Length < pairsNeeded)
        {
            Debug.LogWarning($"Not enough sprites. Need {pairsNeeded}, have {(cardSprites == null ? 0 : cardSprites.Length)}. Grid size not changed.");
            return;
        }

        rows = newRows;
        columns = newColumns;
        InitializeNewGame();
    }

    public void ResetGame()
    {
        gameActive = false;
        InitializeNewGame();
    }

    public List<int> GetMatchedCardIds()
    {
        return matchedCardIds;
    }

    public bool IsGameActive => gameActive;
}

