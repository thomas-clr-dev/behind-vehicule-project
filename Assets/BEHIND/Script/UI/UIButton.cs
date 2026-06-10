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
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
    }

    private void AnimateToHover()
    {
        rectTransform.DOKill();
        image.DOKill();
        rectTransform.DOScale(hoverScale, scaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
        image.DOColor(hoverColor, colorDuration).SetUpdate(true);
    }

    private void AnimateToNormal()
    {
        rectTransform.DOKill();
        image.DOKill();
        rectTransform.DOScale(normalScale, scaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
        image.DOColor(normalColor, colorDuration).SetUpdate(true);
    }

    private void AnimateToPressed()
    {
        rectTransform.DOKill();
        image.DOKill();
        rectTransform.DOScale(pressedScale, scaleDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        image.DOColor(pressedColor, colorDuration).SetUpdate(true);
    }

    // --- SOURIS ---
    public void OnPointerEnter(PointerEventData eventData) => AnimateToHover();
    public void OnPointerExit(PointerEventData eventData) => AnimateToNormal();
    public void OnPointerDown(PointerEventData eventData) => AnimateToPressed();
    public void OnPointerUp(PointerEventData eventData) => AnimateToHover();

    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.DOKill();
        rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.5f).SetUpdate(true);
    }

    // --- MANETTE / CLAVIER ---
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Selected via keyboard/gamepad");
        AnimateToHover();
    }

    public void OnDeselect(BaseEventData eventData) => AnimateToNormal();

    public void OnSubmit(BaseEventData eventData)
    {
        rectTransform.DOKill();
        rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.5f).SetUpdate(true);
    }

    private void OnDisable()
    {
        rectTransform.DOKill();
        image.DOKill();
        rectTransform.localScale = Vector3.one * normalScale;
        image.color = normalColor;
    }
}