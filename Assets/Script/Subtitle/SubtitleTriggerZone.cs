using UnityEngine;

/// <summary>
/// Zone qui déclenche l'affichage d'un sous-titre quand le joueur entre
/// </summary>
public class SubtitleTriggerZone : MonoBehaviour
{
    #region SerializeField
    [Header("Subtitle Settings")]
    [Tooltip("Données du sous-titre à afficher")]
    [SerializeField] private SubtitleData _subtitleData;

    [Header("Trigger Behavior")]
    [Tooltip("Cacher le sous-titre quand le joueur sort de la zone")]
    [SerializeField] private bool _hideOnExit = false;

    [Tooltip("Activer seulement une fois")]
    [SerializeField] private bool _triggerOnce = true;

    [Tooltip("Délai avant d'afficher le sous-titre (en secondes)")]
    [SerializeField] private float _delay = 0f;

    [Header("Debug")]
    [Tooltip("Afficher les logs de debug")]
    [SerializeField] private bool _showDebugLogs = false;
    #endregion

    #region Private Fields
    private bool _hasTriggered = false;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_triggerOnce && _hasTriggered)
                return;

            if (_subtitleData == null)
            {
                Debug.LogWarning($"SubtitleTriggerZone '{gameObject.name}': SubtitleData is null!");
                return;
            }

            if (SubtitleManager.Instance == null)
            {
                Debug.LogWarning($"SubtitleTriggerZone '{gameObject.name}': SubtitleManager not found in scene!");
                return;
            }

            _hasTriggered = true;

            if (_showDebugLogs)
            {
                Debug.Log($"SubtitleTriggerZone '{gameObject.name}': Player entered, showing subtitle");
            }

            // Déclenche l'affichage avec ou sans délai
            if (_delay > 0f)
            {
                Invoke(nameof(ShowSubtitle), _delay);
            }
            else
            {
                ShowSubtitle();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _hideOnExit)
        {
            if (_showDebugLogs)
            {
                Debug.Log($"SubtitleTriggerZone '{gameObject.name}': Player exited, hiding subtitle");
            }

            if (SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.HideSubtitle();
            }
        }
    }
    #endregion

    #region Private Methods
    private void ShowSubtitle()
    {
        Debug.Log($"🎯 SubtitleTriggerZone.ShowSubtitle() - Manager exists: {SubtitleManager.Instance != null}"); // ← AJOUTEZ CECI
    
        if (SubtitleManager.Instance != null)
        {
            Debug.Log($"🎯 Calling ShowSubtitle with text: {_subtitleData?.Text ?? "NULL"}"); // ← AJOUTEZ CECI
            SubtitleManager.Instance.ShowSubtitle(_subtitleData);
        }
        else
        {
            Debug.LogError("🎯 SubtitleManager.Instance is NULL!"); // ← AJOUTEZ CECI
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Force l'affichage du sous-titre (peut être appelé manuellement)
    /// </summary>
    public void ForceShowSubtitle()
    {
        if (_subtitleData != null && SubtitleManager.Instance != null)
        {
            ShowSubtitle();
        }
    }

    /// <summary>
    /// Réinitialise le trigger pour permettre de le réutiliser
    /// </summary>
    public void ResetTrigger()
    {
        _hasTriggered = false;
    }
    #endregion
}