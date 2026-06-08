using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    ISelectHandler,
    IDeselectHandler,
    ISubmitHandler
{
    [Header("Références")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;

    [Header("Animation Scale")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float scaleDuration = 0.15f;

    [Header("Animation Couleur")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color pressedColor = Color.gray;
    [SerializeField] private float colorDuration = 0.1f;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();
    }

    // --- SOURIS ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOScale(hoverScale, scaleDuration).SetEase(Ease.OutBack);
        image.DOColor(hoverColor, colorDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(normalScale, scaleDuration).SetEase(Ease.OutBack);
        image.DOColor(normalColor, colorDuration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.DOScale(pressedScale, scaleDuration).SetEase(Ease.OutQuad);
        image.DOColor(pressedColor, colorDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rectTransform.DOScale(hoverScale, scaleDuration).SetEase(Ease.OutBack);
        image.DOColor(hoverColor, colorDuration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Petit punch au click
        rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.5f);
    }

    // --- MANETTE / CLAVIER ---

    public void OnSelect(BaseEventData eventData)
    {
        // Même effet que hover
        rectTransform.DOScale(hoverScale, scaleDuration).SetEase(Ease.OutBack);
        image.DOColor(hoverColor, colorDuration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        rectTransform.DOScale(normalScale, scaleDuration).SetEase(Ease.OutBack);
        image.DOColor(normalColor, colorDuration);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        // Confirmé avec A / Espace / Entrée
        rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.5f);
    }

    private void OnDisable()
    {
        // Reset propre quand le bouton est désactivé
        rectTransform.DOKill();
        image.DOKill();
        rectTransform.localScale = Vector3.one * normalScale;
        image.color = normalColor;
    }
}