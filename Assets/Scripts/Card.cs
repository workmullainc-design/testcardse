using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;
    [SerializeField] private Button cardButton;
    [SerializeField] private float flipDuration = 0.3f;

    private int cardId;
    private bool isFlipped = false;
    private bool isMatched = false;
    private CanvasGroup canvasGroup;
    private bool isAnimating = false;

    public int CardId => cardId;
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        if (cardButton == null)
        {
            cardButton = GetComponent<Button>();
        }

        if (frontImage == null || backImage == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
            {
                string lowerName = img.name.ToLower();
                if (frontImage == null && lowerName.Contains("front"))
                {
                    frontImage = img;
                }
                else if (backImage == null && lowerName.Contains("back"))
                {
                    backImage = img;
                }
            }

            if (frontImage == null || backImage == null)
            {
                foreach (Image img in images)
                {
                    if (frontImage == null && img != backImage)
                    {
                        frontImage = img;
                    }
                    else if (backImage == null && img != frontImage)
                    {
                        backImage = img;
                    }
                }
            }
        }

        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
        }
        else
        {
            Debug.LogWarning("Card: Button not assigned or found.", this);
        }

        if (frontImage == null || backImage == null)
        {
            Debug.LogWarning("Card: Front/Back images not assigned or found.", this);
        }
    }

    public void Initialize(int id, Sprite cardSprite)
    {
        cardId = id;
        
        if (frontImage != null && cardSprite != null)
        {
            frontImage.sprite = cardSprite;
            Debug.Log($"Card {id}: Set front sprite to {cardSprite.name}");
        }
        else
        {
            Debug.LogError($"Card {id}: frontImage is null or sprite is null. frontImage: {frontImage}, sprite: {cardSprite}");
        }
        
        isFlipped = false;
        isMatched = false;
        isAnimating = false;
        canvasGroup.alpha = 1f;
        UpdateVisuals();
    }

    public void OnCardClicked()
    {
        if (!isFlipped && !isMatched && !isAnimating)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCardClick();
            }
            GameManager.Instance.OnCardFlipped(this);
        }
    }

    public void Flip()
    {
        if (isAnimating)
            return;

        StartCoroutine(FlipCoroutine());
    }

    private IEnumerator FlipCoroutine()
    {
        isAnimating = true;
        float elapsed = 0f;

        // Flip to back/front
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (flipDuration / 2);
            transform.localScale = new Vector3(1 - progress, 1, 1);
            yield return null;
        }

        isFlipped = !isFlipped;
        UpdateVisuals();

        elapsed = 0f;
        // Flip back
        while (elapsed < flipDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (flipDuration / 2);
            transform.localScale = new Vector3(progress, 1, 1);
            yield return null;
        }

        transform.localScale = Vector3.one;
        isAnimating = false;
    }

    public void SetMatched()
    {
        isMatched = true;
        Debug.Log($"Card {cardId} marked as matched, starting fade out");
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;
            canvasGroup.alpha = 1f - progress;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        cardButton.interactable = false;
        Debug.Log($"Card {cardId} fade completed");
    }

    public void ResetFlip()
    {
        if (!isMatched && isFlipped)
        {
            StartCoroutine(FlipCoroutine());
        }
    }

    private void UpdateVisuals()
    {
        if (frontImage == null || backImage == null)
        {
            Debug.LogWarning($"Card {cardId}: UpdateVisuals - frontImage or backImage is null");
            return;
        }

        frontImage.gameObject.SetActive(isFlipped);
        backImage.gameObject.SetActive(!isFlipped);
        
        Debug.Log($"Card {cardId}: UpdateVisuals - isFlipped={isFlipped}, front active={isFlipped}, back active={!isFlipped}");
    }

    public void ResetCard()
    {
        isFlipped = false;
        isMatched = false;
        isAnimating = false;
        canvasGroup.alpha = 1f;
        cardButton.interactable = true;
        UpdateVisuals();
    }
}
