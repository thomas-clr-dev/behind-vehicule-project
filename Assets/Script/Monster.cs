using UnityEngine;

public class Monster : MonoBehaviour
{
    #region SerializeField
    [Header("Chase Settings")]
    [Tooltip("Vitesse de déplacement du monstre")]
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("References")]
    [Tooltip("Référence au script de calcul de distance")]
    [SerializeField] private DistanceFromMonsterToPlayer _distanceCalculator;

    [Tooltip("Zone de déclenchement de la chase")]
    [SerializeField] private ChaseTriggerZone _chaseTriggerZone;

    [Tooltip("Zone(s) de fin de la chase")]
    [SerializeField] private ChaseEndZone _endZone;
    #endregion

    #region Private Fields
    private bool _isChasing = false;
    private Renderer[] _renderers;
    private Vector3 _initialPosition;
    #endregion

    #region Initialization
    private void Start()
    {
        Debug.Log($"🔵 Monster '{gameObject.name}' - Start()");

        // Sauvegarder la position initiale
        _initialPosition = transform.position;

        // Récupérer tous les renderers
        _renderers = GetComponentsInChildren<Renderer>();
        Debug.Log($"👁️ Monster '{gameObject.name}' - {_renderers.Length} Renderer(s) trouvé(s)");

        // S'abonner à la zone de START
        if (_chaseTriggerZone != null)
        {
            Debug.Log($"✅ Monster '{gameObject.name}' - ChaseTriggerZone assignée: {_chaseTriggerZone.gameObject.name}");
            _chaseTriggerZone.OnChaseBegin += StartChasing;
        }
        else
        {
            Debug.LogWarning($"⚠️ Monster '{gameObject.name}' - ChaseTriggerZone non assignée, recherche...");
            _chaseTriggerZone = FindObjectOfType<ChaseTriggerZone>();
            if (_chaseTriggerZone != null)
            {
                Debug.Log($"✅ Monster '{gameObject.name}' - ChaseTriggerZone trouvée: {_chaseTriggerZone.gameObject.name}");
                _chaseTriggerZone.OnChaseBegin += StartChasing;
            }
            else
            {
                Debug.LogError($"❌ Monster '{gameObject.name}' - Aucune ChaseTriggerZone trouvée!");
            }
        }

        // S'abonner aux zones de END
        if (_endZone != null)
        {
            if (_endZone != null)
            {
                _endZone.OnChaseEnd += StopChasing;
                Debug.Log($"✅ Monster '{gameObject.name}' - Abonné à EndZone: {_endZone.gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Monster '{gameObject.name}' - EndZones non assignées, recherche...");
            _endZone = FindObjectOfType<ChaseEndZone>();
            if (_endZone != null)
            {
                _endZone.OnChaseEnd += StopChasing;
                Debug.Log($"✅ Monster '{gameObject.name}' - Abonné à EndZone trouvée: {_endZone.gameObject.name}");
            }
            else
            {
                Debug.LogError($"❌ Monster '{gameObject.name}' - Aucune ChaseEndZone trouvée!");
            }
        }

        // Vérifier la référence au distance calculator
        if (_distanceCalculator == null)
        {
            _distanceCalculator = GetComponent<DistanceFromMonsterToPlayer>();

            if (_distanceCalculator == null)
            {
                Debug.LogWarning("⚠️ DistanceFromMonsterToPlayer non trouvé sur le monstre!");
            }
        }

        // Rendre invisible au démarrage
        SetVisibility(false);
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_isChasing && _distanceCalculator != null)
        {
            // Calculer le déplacement selon l'axe configuré
            Vector3 movement = Vector3.zero;

            switch (_distanceCalculator.Axis)
            {
                case MovementAxis.X:
                    movement = new Vector3(_chaseSpeed * Time.deltaTime, 0, 0);
                    break;
                case MovementAxis.Y:
                    movement = new Vector3(0, _chaseSpeed * Time.deltaTime, 0);
                    break;
                case MovementAxis.Z:
                    movement = new Vector3(0, 0, _chaseSpeed * Time.deltaTime);
                    break;
            }

            transform.position += movement;
        }
    }
    #endregion

    #region Chase Logic
    private void StartChasing()
    {
        Debug.Log("🏃 Monster started chasing!");

        _isChasing = true;

        // Rendre visible
        SetVisibility(true);
    }

    public void StopChasing()
    {
        Debug.Log("🛑 Monster stopped chasing!");

        // Arrêter le déplacement
        _isChasing = false;

        // Rendre invisible
        SetVisibility(false);

        // Optionnel : Réinitialiser la position
        ResetPosition();
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Rend le monstre visible ou invisible
    /// </summary>
    private void SetVisibility(bool visible)
    {
        if (_renderers != null && _renderers.Length > 0)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = visible;
                }
            }

            Debug.Log($"👁️ Monster visibility: {(visible ? "VISIBLE" : "INVISIBLE")}");
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun Renderer trouvé sur le monstre!");
        }
    }

    /// <summary>
    /// Réinitialise la position du monstre à sa position initiale
    /// </summary>
    private void ResetPosition()
    {
        transform.position = _initialPosition;
        Debug.Log($"🔄 Monster position reset to {_initialPosition}");
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        // Désabonnement des zones de START
        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin -= StartChasing;
        }

        // Désabonnement des zones de END
        if (_endZone != null)
        {
            _endZone.OnChaseEnd -= StopChasing;
        }
    #endregion
    }
}