using UnityEngine;

public class Monster : MonoBehaviour
{
    #region SerializeField
    [Header("Chase Settings")]
    [Tooltip("Vitesse de base du monstre (non utilisée avec vitesse adaptative)")]
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("Adaptive Speed Settings")]
    [Tooltip("Vitesse quand très loin du joueur")]
    [SerializeField] private float _speedVeryFar = 6f;
    
    [Tooltip("Vitesse quand loin du joueur")]
    [SerializeField] private float _speedFar = 5f;
    
    [Tooltip("Vitesse quand à distance moyenne du joueur")]
    [SerializeField] private float _speedMid = 4f;
    
    [Tooltip("Vitesse quand proche du joueur")]
    [SerializeField] private float _speedNear = 3.5f;
    
    [Tooltip("Vitesse quand très proche du joueur")]
    [SerializeField] private float _speedVeryClose = 3f;

    [Header("Distance Thresholds")]
    [Tooltip("Distance pour être considéré comme 'Very Far'")]
    [SerializeField] private float _distanceVeryFar = 20f;
    
    [Tooltip("Distance pour être considéré comme 'Far'")]
    [SerializeField] private float _distanceFar = 15f;
    
    [Tooltip("Distance pour être considéré comme 'Mid'")]
    [SerializeField] private float _distanceMid = 10f;
    
    [Tooltip("Distance pour être considéré comme 'Near'")]
    [SerializeField] private float _distanceNear = 5f;
    
    [Tooltip("Distance pour être considéré comme 'Very Close' (< cette valeur)")]
    [SerializeField] private float _distanceVeryClose = 3f;

    [Header("Speed Transition")]
    [Tooltip("Vitesse de transition entre les paliers (Lerp)")]
    [SerializeField] private float _speedTransitionSpeed = 2f;

    [Header("References")]
    [Tooltip("Référence au script de calcul de distance")]
    [SerializeField] private DistanceFromMonsterToPlayer _distanceCalculator;

    [Tooltip("Zone de déclenchement de la chase")]
    [SerializeField] private ChaseTriggerZone _chaseTriggerZone;

    [Tooltip("Zone de fin de la chase")]
    [SerializeField] private ChaseEndZone _endZone;
    #endregion

    #region Private Fields
    private bool _isChasing = false;
    private Renderer[] _renderers;
    private Vector3 _initialPosition;
    private float _currentSpeed;
    private MonsterSpeedState _currentSpeedState;
    #endregion

    #region Public Properties
    public bool IsChasing => _isChasing;
    #endregion

    #region Initialization
    private void Start()
    {
        Debug.Log($"🔵 Monster '{gameObject.name}' - Start()");

        // Sauvegarder la position initiale
        _initialPosition = transform.position;

        // Initialiser la vitesse
        _currentSpeed = _speedVeryFar;

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
            Debug.LogError($"❌ Monster '{gameObject.name}' - ChaseTriggerZone NON ASSIGNÉE!");
        }

        // S'abonner à la zone de END
        if (_endZone != null)
        {
            _endZone.OnChaseEnd += StopChasing;
            Debug.Log($"✅ Monster '{gameObject.name}' - Abonné à EndZone: {_endZone.gameObject.name}");
        }
        else
        {
            Debug.LogError($"❌ Monster '{gameObject.name}' - EndZone NON ASSIGNÉE!");
        }

        // Vérifier la référence au distance calculator
        if (_distanceCalculator == null)
        {
            _distanceCalculator = GetComponent<DistanceFromMonsterToPlayer>();

            if (_distanceCalculator == null)
            {
                Debug.LogWarning($"⚠️ Monster '{gameObject.name}' - DistanceFromMonsterToPlayer non trouvé!");
            }
            else
            {
                Debug.Log($"✅ Monster '{gameObject.name}' - DistanceFromMonsterToPlayer trouvé");
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
            // Mettre à jour la vitesse en fonction de la distance
            UpdateSpeedBasedOnDistance();

            // Calculer le déplacement selon l'axe configuré
            Vector3 movement = Vector3.zero;

            switch (_distanceCalculator.Axis)
            {
                case MovementAxis.X:
                    movement = new Vector3(_currentSpeed * Time.deltaTime, 0, 0);
                    break;
                case MovementAxis.Y:
                    movement = new Vector3(0, _currentSpeed * Time.deltaTime, 0);
                    break;
                case MovementAxis.Z:
                    movement = new Vector3(0, 0, _currentSpeed * Time.deltaTime);
                    break;
                case MovementAxis.NegX:
                    movement = new Vector3(-_currentSpeed * Time.deltaTime, 0, 0);
                    break;
                case MovementAxis.NegY:
                    movement = new Vector3(0, -_currentSpeed * Time.deltaTime, 0);
                    break;
                case MovementAxis.NegZ:
                    movement = new Vector3(0, 0, -_currentSpeed * Time.deltaTime);
                    break;
            }

            transform.position += movement;
        }
    }
    #endregion

    #region Speed Management
    /// <summary>
    /// Met à jour la vitesse du monstre en fonction de la distance au joueur
    /// </summary>
    private void UpdateSpeedBasedOnDistance()
    {
        if (_distanceCalculator == null) return;

        float distance = _distanceCalculator.Distance;
        float targetSpeed = _speedVeryFar;
        MonsterSpeedState newState = MonsterSpeedState.VeryFar;

        // Évaluer le palier de vitesse en fonction de la distance
        if (distance >= _distanceVeryFar)
        {
            targetSpeed = _speedVeryFar;
            newState = MonsterSpeedState.VeryFar;
        }
        else if (distance >= _distanceFar)
        {
            targetSpeed = _speedFar;
            newState = MonsterSpeedState.Far;
        }
        else if (distance >= _distanceMid)
        {
            targetSpeed = _speedMid;
            newState = MonsterSpeedState.Mid;
        }
        else if (distance >= _distanceNear)
        {
            targetSpeed = _speedNear;
            newState = MonsterSpeedState.Near;
        }
        else
        {
            targetSpeed = _speedVeryClose;
            newState = MonsterSpeedState.VeryClose;
        }

        // Log le changement d'état
        if (newState != _currentSpeedState)
        {
            Debug.Log($"🏃 Monster '{gameObject.name}' - Speed state: {_currentSpeedState} → {newState} (Distance: {distance:F2}, Speed: {targetSpeed:F2})");
            _currentSpeedState = newState;
        }

        // Transition fluide vers la vitesse cible
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _speedTransitionSpeed * Time.deltaTime);
    }
    #endregion

    #region Chase Logic
    private void StartChasing()
    {
        Debug.Log($"🏃 Monster '{gameObject.name}' started chasing!");

        _isChasing = true;

        // Rendre visible
        SetVisibility(true);
    }

    public void StopChasing()
    {
        Debug.Log($"🛑 Monster '{gameObject.name}' stopped chasing!");

        // Arrêter le déplacement
        _isChasing = false;

        // Réinitialiser la vitesse
        _currentSpeed = _speedVeryFar;
        _currentSpeedState = MonsterSpeedState.VeryFar;

        // Rendre invisible
        SetVisibility(false);

        // Réinitialiser la position
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

            Debug.Log($"👁️ Monster '{gameObject.name}' visibility: {(visible ? "VISIBLE" : "INVISIBLE")}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Monster '{gameObject.name}' - Aucun Renderer trouvé!");
        }
    }

    /// <summary>
    /// Réinitialise la position du monstre à sa position initiale
    /// </summary>
    private void ResetPosition()
    {
        transform.position = _initialPosition;
        Debug.Log($"🔄 Monster '{gameObject.name}' position reset to {_initialPosition}");
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        // Désabonnement de la zone de START
        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin -= StartChasing;
        }

        // Désabonnement de la zone de END
        if (_endZone != null)
        {
            _endZone.OnChaseEnd -= StopChasing;
        }
    }
    #endregion
}

/// <summary>
/// État de vitesse du monstre selon la distance
/// </summary>
public enum MonsterSpeedState
{
    VeryFar,   // > 20 unités
    Far,       // 15-20 unités
    Mid,       // 10-15 unités
    Near,      // 5-10 unités
    VeryClose  // < 5 unités
}