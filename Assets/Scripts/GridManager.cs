using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    [SerializeField] private float cardSpacing = 10f;
    [SerializeField] private float maxCardSize = 353.3f;

    private GridLayoutGroup gridLayoutGroup;
    private Card[] cards;

    public Card[] Cards => cards;
    public int Rows => rows;
    public int Columns => columns;

    private void Awake()
    {
        if (gridContainer == null)
            gridContainer = GetComponent<RectTransform>();

        gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null)
            gridLayoutGroup = gridContainer.gameObject.AddComponent<GridLayoutGroup>();
    }

    public void InitializeGrid(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;

        // Clear existing cards
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // Configure grid layout
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
        gridLayoutGroup.spacing = new Vector2(cardSpacing, cardSpacing);

        // Calculate card size based on container
        CalculateCardSize();

        // Create card grid
        cards = new Card[rows * columns];
        for (int i = 0; i < rows * columns; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridContainer);
            cards[i] = cardObj.GetComponent<Card>();
        }
    }

    private void CalculateCardSize()
    {
        RectTransform containerRect = gridContainer;
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // Calculate size accounting for spacing
        float totalHorizontalSpacing = (columns - 1) * cardSpacing;
        float totalVerticalSpacing = (rows - 1) * cardSpacing;

        float cardWidth = (containerWidth - totalHorizontalSpacing) / columns;
        float cardHeight = (containerHeight - totalVerticalSpacing) / rows;

        // Use the smaller dimension to maintain aspect ratio
        float cardSize = Mathf.Min(cardWidth, cardHeight);
        
        // Cap to maximum size
        cardSize = Mathf.Min(cardSize, maxCardSize);

        gridLayoutGroup.cellSize = new Vector2(cardSize, cardSize);
    }

    public void ResetAllCards()
    {
        foreach (Card card in cards)
        {
            card.ResetCard();
        }
    }

    public Card GetCard(int index)
    {
        if (index >= 0 && index < cards.Length)
            return cards[index];
        return null;
    }

    public int GetTotalCards()
    {
        return rows * columns;
    }
}
