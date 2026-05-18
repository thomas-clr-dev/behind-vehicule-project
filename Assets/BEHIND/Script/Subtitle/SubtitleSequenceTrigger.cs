using System.Collections;
using UnityEngine;

/// <summary>
/// Déclenche une séquence de sous-titres
/// </summary>
public class SubtitleSequenceTrigger : MonoBehaviour
{
    #region SerializeField
    [Header("Sequence Settings")]
    [SerializeField] private SubtitleSequence _sequence;
    
    [Header("Trigger Behavior")]
    [SerializeField] private bool _triggerOnce = true;
    [SerializeField] private bool _autoPlayOnStart = false;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLogs = false;
    #endregion

    #region Private Fields
    private bool _hasTriggered = false;
    private Coroutine _sequenceCoroutine;
    private int _currentSubtitleIndex = 0;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        // S'abonne à l'événement de complétion
        SubtitleManager.OnSubtitleCompleted += OnSubtitleCompleted;
    }

    private void OnDisable()
    {
        // Se désabonne
        SubtitleManager.OnSubtitleCompleted -= OnSubtitleCompleted;
    }

    private void Start()
    {
        if (_autoPlayOnStart)
        {
            PlaySequence();
        }
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_triggerOnce && _hasTriggered)
                return;

            PlaySequence();
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Lance la séquence de sous-titres
    /// </summary>
    public void PlaySequence()
    {
        if (_sequence == null || _sequence.Subtitles.Count == 0)
        {
            Debug.LogWarning("SubtitleSequenceTrigger: No sequence or empty sequence!");
            return;
        }

        if (SubtitleManager.Instance == null)
        {
            Debug.LogWarning("SubtitleSequenceTrigger: SubtitleManager not found in scene!");
            return;
        }

        _hasTriggered = true;

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
        }

        _currentSubtitleIndex = 0;
        _sequenceCoroutine = StartCoroutine(PlaySequenceCoroutine());
    }

    /// <summary>
    /// Arrête la séquence en cours
    /// </summary>
    public void StopSequence()
    {
        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
        }

        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.HideSubtitle();
        }
    }

    /// <summary>
    /// Réinitialise le trigger
    /// </summary>
    public void ResetTrigger()
    {
        _hasTriggered = false;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Coroutine qui joue la séquence
    /// </summary>
    private IEnumerator PlaySequenceCoroutine()
    {
        if (_showDebugLogs)
        {
            Debug.Log($"SubtitleSequenceTrigger: Starting sequence with {_sequence.Subtitles.Count} subtitles");
        }

        for (_currentSubtitleIndex = 0; _currentSubtitleIndex < _sequence.Subtitles.Count; _currentSubtitleIndex++)
        {
            SubtitleData subtitle = _sequence.Subtitles[_currentSubtitleIndex];
            
            if (subtitle == null)
            {
                if (_showDebugLogs)
                {
                    Debug.LogWarning($"SubtitleSequenceTrigger: Subtitle at index {_currentSubtitleIndex} is null, skipping");
                }
                continue;
            }

            // Affiche le sous-titre
            SubtitleManager.Instance.ShowSubtitle(subtitle);

            // Attend la durée du sous-titre + délai
            float waitTime = subtitle.GetEffectiveDuration() + _sequence.DelayBetweenSubtitles;
            yield return new WaitForSeconds(waitTime);
        }

        if (_showDebugLogs)
        {
            Debug.Log("SubtitleSequenceTrigger: Sequence completed");
        }

        _sequenceCoroutine = null;
    }

    /// <summary>
    /// Appelé quand un sous-titre est complété
    /// </summary>
    private void OnSubtitleCompleted(SubtitleData completedSubtitle)
    {
        if (_showDebugLogs)
        {
            Debug.Log($"SubtitleSequenceTrigger: Subtitle completed - {completedSubtitle.Text.Substring(0, Mathf.Min(20, completedSubtitle.Text.Length))}...");
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
    #endregion
}