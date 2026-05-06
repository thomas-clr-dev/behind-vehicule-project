using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Gestionnaire central des sous-titres utilisant des événements C# classiques
/// </summary>
public class SubtitleManager : MonoBehaviour
{
    #region Singleton
    public static SubtitleManager Instance { get; private set; }
    #endregion

    #region Events
    /// <summary>
    /// Événement déclenché quand un sous-titre est complété
    /// </summary>
    public static event Action<SubtitleData> OnSubtitleCompleted;

    /// <summary>
    /// Événement déclenché quand un sous-titre commence à s'afficher
    /// </summary>
    public static event Action<SubtitleData> OnSubtitleStarted;

    /// <summary>
    /// Événement déclenché quand les sous-titres sont cachés
    /// </summary>
    public static event Action OnSubtitleHidden;
    #endregion

    #region SerializeField
    [Header("UI References")]
    [Tooltip("Panel contenant les sous-titres")]
    [SerializeField] private GameObject _subtitlePanel;

    [Tooltip("TextMeshPro pour le texte du sous-titre")]
    [SerializeField] private TextMeshProUGUI _subtitleText;

    [Tooltip("TextMeshPro pour le nom du personnage (optionnel)")]
    [SerializeField] private TextMeshProUGUI _speakerNameText;

    [Header("Audio")]
    [Tooltip("AudioSource pour la voix")]
    [SerializeField] private AudioSource _voiceAudioSource;

    [Header("Animation Settings")]
    [Tooltip("Animer le texte caractère par caractère")]
    [SerializeField] private bool _useTypewriterEffect = false;

    [Tooltip("Vitesse de l'effet machine à écrire (caractères par seconde)")]
    [SerializeField] private float _typewriterSpeed = 30f;

    [Header("Fade Settings")]
    [Tooltip("Utiliser un effet de fade in/out")]
    [SerializeField] private bool _useFadeEffect = true;

    [Tooltip("Durée du fade in/out")]
    [SerializeField] private float _fadeDuration = 0.3f;
    #endregion

    #region Private Fields
    private Coroutine _currentSubtitleCoroutine;
    private CanvasGroup _canvasGroup;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Setup CanvasGroup pour le fade
        if (_useFadeEffect && _subtitlePanel != null)
        {
            _canvasGroup = _subtitlePanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = _subtitlePanel.AddComponent<CanvasGroup>();
            }
        }

        // Cache les sous-titres au démarrage
        HideSubtitleImmediate();
    }

    private void OnDestroy()
    {
        // Nettoie les événements statiques
        OnSubtitleCompleted = null;
        OnSubtitleStarted = null;
        OnSubtitleHidden = null;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Affiche un sous-titre
    /// </summary>
    public void ShowSubtitle(SubtitleData data)
    {
        if (data == null)
        {
            Debug.LogWarning("SubtitleManager: SubtitleData is null!");
            return;
        }

        // Arrête le sous-titre précédent si existant
        if (_currentSubtitleCoroutine != null)
        {
            StopCoroutine(_currentSubtitleCoroutine);
        }

        _currentSubtitleCoroutine = StartCoroutine(DisplaySubtitleCoroutine(data));
    }

    /// <summary>
    /// Cache les sous-titres avec animation
    /// </summary>
    public void HideSubtitle()
    {
        if (_currentSubtitleCoroutine != null)
        {
            StopCoroutine(_currentSubtitleCoroutine);
        }

        _currentSubtitleCoroutine = StartCoroutine(HideSubtitleCoroutine());
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Coroutine principale d'affichage du sous-titre
    /// </summary>
    private IEnumerator DisplaySubtitleCoroutine(SubtitleData data)
    {
        // Déclenche l'événement de début
        OnSubtitleStarted?.Invoke(data);

        // Configure le texte
        if (_speakerNameText != null && !string.IsNullOrEmpty(data.SpeakerName))
        {
            _speakerNameText.text = data.SpeakerName;
            _speakerNameText.gameObject.SetActive(true);
        }
        else if (_speakerNameText != null)
        {
            _speakerNameText.gameObject.SetActive(false);
        }

        // Configure la couleur
        if (_subtitleText != null)
        {
            _subtitleText.color = data.TextColor;
        }

        // Joue l'audio si présent
        if (_voiceAudioSource != null && data.VoiceClip != null)
        {
            _voiceAudioSource.clip = data.VoiceClip;
            _voiceAudioSource.Play();
        }

        // Affiche le panel
        if (_subtitlePanel != null)
        {
            _subtitlePanel.SetActive(true);
        }

        // Fade in
        if (_useFadeEffect && _canvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(_canvasGroup, 0f, 1f, _fadeDuration));
        }

        // Affiche le texte (avec ou sans effet typewriter)
        if (_useTypewriterEffect)
        {
            yield return StartCoroutine(TypewriterEffect(data.Text));
        }
        else
        {
            if (_subtitleText != null)
            {
                _subtitleText.text = data.Text;
            }
        }

        // Attend la durée spécifiée
        float duration = data.GetEffectiveDuration();
        
        // Si un audio est présent, utilise sa durée si elle est plus longue
        if (_voiceAudioSource != null && data.VoiceClip != null)
        {
            duration = Mathf.Max(duration, data.VoiceClip.length);
        }

        yield return new WaitForSeconds(duration);

        // Déclenche l'événement de complétion
        OnSubtitleCompleted?.Invoke(data);

        // Cache le sous-titre
        yield return StartCoroutine(HideSubtitleCoroutine());

        _currentSubtitleCoroutine = null;
    }

    /// <summary>
    /// Coroutine pour cacher le sous-titre avec fade
    /// </summary>
    private IEnumerator HideSubtitleCoroutine()
    {
        // Arrête l'audio
        if (_voiceAudioSource != null && _voiceAudioSource.isPlaying)
        {
            _voiceAudioSource.Stop();
        }

        // Fade out
        if (_useFadeEffect && _canvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(_canvasGroup, 1f, 0f, _fadeDuration));
        }

        // Cache le panel
        HideSubtitleImmediate();

        // Déclenche l'événement
        OnSubtitleHidden?.Invoke();
    }

    /// <summary>
    /// Cache immédiatement les sous-titres sans animation
    /// </summary>
    private void HideSubtitleImmediate()
    {
        if (_subtitlePanel != null)
        {
            _subtitlePanel.SetActive(false);
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }

        if (_subtitleText != null)
        {
            _subtitleText.text = "";
        }
    }

    /// <summary>
    /// Effet machine à écrire
    /// </summary>
    private IEnumerator TypewriterEffect(string text)
    {
        if (_subtitleText == null) yield break;

        _subtitleText.text = "";
        float delay = 1f / _typewriterSpeed;

        foreach (char c in text)
        {
            _subtitleText.text += c;
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Fade d'un CanvasGroup
    /// </summary>
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            cg.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        cg.alpha = end;
    }
    #endregion
}
