using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, IEventListener<GameEngineEvent>
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

    private bool _isEnabled = true;
    #endregion

    #region Private Fields
    private bool _isChasing = false;
    private Renderer[] _renderers;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _currentSpeed;
    private MonsterSpeedState _currentSpeedState;
    private MovementAxis _currentDirection;

    // Champs pour la rotation
    private bool _isRotating = false;
    private float _rotationSpeedMultiplier = 1f;
    private Coroutine _rotationCoroutine;
    #endregion

    #region Public Properties
    public bool IsChasing => _isChasing;

    public static event Action OnGameOver;
    #endregion

    #region Initialization
    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        _currentSpeed = _speedVeryFar;

        _renderers = GetComponentsInChildren<Renderer>();

        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin += StartChasing;
        }

        if (_endZone != null)
        {
            _endZone.OnChaseEnd += StopChasing;
        }

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
                _currentDirection = _distanceCalculator.Axis;
            }
        }
        else
        {
            _currentDirection = _distanceCalculator.Axis;
        }

        SubscribeToDirectionZones();

        //SetVisibility(false);
    }

    /// <summary>
    /// S'abonne à toutes les ChangeDirectionChaseTriggerZone de la scène
    /// </summary>
    private void SubscribeToDirectionZones()
    {
        ChangeDirectionChaseTriggerZone[] directionZones = FindObjectsByType<ChangeDirectionChaseTriggerZone>(FindObjectsSortMode.None);

        foreach (var zone in directionZones)
        {
            zone.OnDirectionChange += OnDirectionChanged;
        }
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_isChasing && _distanceCalculator != null)
        {
            UpdateSpeedBasedOnDistance();
            Vector3 movement = Vector3.zero;

            float effectiveSpeed = _currentSpeed * _rotationSpeedMultiplier;

            switch (_currentDirection)
            {
                case MovementAxis.X:
                    movement = new Vector3(effectiveSpeed * Time.deltaTime, 0, 0);
                    break;
                case MovementAxis.Y:
                    movement = new Vector3(0, effectiveSpeed * Time.deltaTime, 0);
                    break;
                case MovementAxis.Z:
                    movement = new Vector3(0, 0, effectiveSpeed * Time.deltaTime);
                    break;
                case MovementAxis.NegX:
                    movement = new Vector3(-effectiveSpeed * Time.deltaTime, 0, 0);
                    break;
                case MovementAxis.NegY:
                    movement = new Vector3(0, -effectiveSpeed * Time.deltaTime, 0);
                    break;
                case MovementAxis.NegZ:
                    movement = new Vector3(0, 0, -effectiveSpeed * Time.deltaTime);
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

        if (newState != _currentSpeedState)
        {
            _currentSpeedState = newState;
        }

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _speedTransitionSpeed * Time.deltaTime);
    }
    #endregion

    #region Chase Logic
    private void StartChasing()
    {
        if (!_isEnabled) return; // ← bloque si désactivé

        _isChasing = true;
        //SetVisibility(true);
    }

    public void StopChasing()
    {
        _isChasing = false;

        _currentSpeed = _speedVeryFar;
        _currentSpeedState = MonsterSpeedState.VeryFar;

        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = null;
        }
        _isRotating = false;
        _rotationSpeedMultiplier = 1f;

        //SetVisibility(false);

        ResetPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopChasing();
            OnGameOver?.Invoke();
        }
    }
    #endregion

    #region Rotation Logic
    /// <summary>
    /// Coroutine qui effectue la rotation progressive
    /// </summary>
    private IEnumerator RotateToNewDirection(DirectionChangeData changeData)
    {
        _isRotating = true;
        _rotationSpeedMultiplier = changeData.SpeedMultiplier;

        Quaternion startRotation = transform.rotation;

        float angleSign = changeData.RotationDirection == RotationDirection.Clockwise ? -1f : 1f;
        Vector3 rotationAxis = Vector3.up;
        Quaternion targetRotation = startRotation * Quaternion.AngleAxis(changeData.RotationAngle * angleSign, rotationAxis);

        float elapsedTime = 0f;

        while (elapsedTime < changeData.RotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / changeData.RotationDuration);

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;

        _currentDirection = changeData.NewDirection;

        _isRotating = false;
        _rotationSpeedMultiplier = 1f;

        _rotationCoroutine = null;
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
        }
    }

    /// <summary>
    /// Réinitialise la position du monstre à sa position initiale
    /// </summary>
    private void ResetPosition()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin -= StartChasing;
        }

        if (_endZone != null)
        {
            _endZone.OnChaseEnd -= StopChasing;
        }

        ChangeDirectionChaseTriggerZone[] directionZones = FindObjectsByType<ChangeDirectionChaseTriggerZone>(FindObjectsSortMode.None);
        foreach (var zone in directionZones)
        {
            zone.OnDirectionChange -= OnDirectionChanged;
        }
    }
    #endregion

    #region Direction Management
    /// <summary>
    /// Appelé quand le monstre entre dans une zone de changement de direction
    /// </summary>
    private void OnDirectionChanged(DirectionChangeData changeData)
    {
        if (_isChasing && !_isRotating)
        {
            if (_rotationCoroutine != null)
            {
                StopCoroutine(_rotationCoroutine);
            }

            _rotationCoroutine = StartCoroutine(RotateToNewDirection(changeData));
        }
    }


    #endregion


    public void OnEvent(GameEngineEvent e)
    {
        Debug.Log("Monster received event: " + e.EventType);
        switch (e.EventType)
        {
            case GameEngineEventTypes.ActivateEnemy:
                _isEnabled = true;
                break;
            case GameEngineEventTypes.DeactivateEnemy:
                _isEnabled = false;
                StopChasing();
                break;
        }
    }

    private void OnEnable()
    {
        this.EventStartListening<GameEngineEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<GameEngineEvent>();
    }
}

/// <summary>
/// État de vitesse du monstre selon la distance
/// </summary>
public enum MonsterSpeedState
{
    VeryFar,
    Far,
    Mid,
    Near,
    VeryClose
}