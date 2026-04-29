using UnityEngine;

public class ChaseMusicDynamism : MonoBehaviour
{
    #region SerializeField - Distance 
    /// <summary> 
    /// References to the distance calculators of the monsters 
    /// </summary> 
    [SerializeField] private DistanceFromMonsterToPlayer[] _distanceCalculators;
    [SerializeField] private int _distanceForNoChase = 20;
    [SerializeField] private int _distanceForFar = 15;
    [SerializeField] private int _distanceForMid = 10;
    [SerializeField] private int _distanceForNear = 5;
    [SerializeField] private int _distanceForVeryClose = 0;
    #endregion

    #region SerializeField - Chase Triggers
    [Header("Chase Triggers")]
    [Tooltip("Zone(s) de déclenchement de la chase")]
    [SerializeField] private ChaseTriggerZone[] _startZones;

    [Tooltip("Zone(s) de fin de la chase")]
    [SerializeField] private ChaseEndZone[] _endZones;
    #endregion

    #region SerializeField - Audio Layers
    [Header("Audio Layers")]
    [SerializeField] private AudioSource _heartbeat;
    [SerializeField] private AudioSource _percussion;
    [SerializeField] private AudioSource _stringAccent;
    [SerializeField] private AudioSource _stringMelody;
    [SerializeField] private AudioSource _tuba;
    #endregion

    #region SerializeField - Audio Settings
    [Header("Audio Settings - Volume")]
    [Tooltip("Volume quand FAR")]
    [SerializeField] private float _volumeFar = 0.8f;
    [Tooltip("Volume quand MID")]
    [SerializeField] private float _volumeMid = 1f;
    [Tooltip("Volume quand NEAR")]
    [SerializeField] private float _volumeNear = 1.2f;
    [Tooltip("Volume quand VERY CLOSE")]
    [SerializeField] private float _volumeVeryClose = 1.4f;

    [Header("Fade Time")]
    [Tooltip("Durée de la transition linéaire en secondes")]
    [SerializeField] private float _fadeTime = 3f;
    #endregion

    #region Private Fields
    /// <summary>
    /// Distance from the monster to the player
    /// </summary>
    private float _monsterDistance;
    /// <summary>
    /// State of the chase
    /// </summary>
    private ChaseState _currentState;
    /// <summary>
    /// Previous state of the chase
    /// </summary>
    private ChaseState _previousState;
    /// <summary>
    /// Indique si la chase est active
    /// </summary>
    private bool _isChaseActive = false;
    /// <summary>
    /// Indique si on est en train de faire un fade out
    /// </summary>
    private bool _isFadingOut = false;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Start Method
    /// </summary>
    private void Start()
    {
        // Configuration initiale
        SetupAudioSources();

        // S'abonner aux zones de START
        if (_startZones != null && _startZones.Length > 0)
        {
            foreach (var zone in _startZones)
            {
                if (zone != null)
                {
                    zone.OnChaseBegin += OnChaseStarted;
                }
            }
            Debug.Log($"✅ Abonné à {_startZones.Length} zone(s) de START");
        }

        // S'abonner aux zones de END
        if (_endZones != null && _endZones.Length > 0)
        {
            foreach (var zone in _endZones)
            {
                if (zone != null)
                {
                    zone.OnChaseEnd += OnChaseEnded;
                }
            }
            Debug.Log($"✅ Abonné à {_endZones.Length} zone(s) de END");
        }
    }

    private void OnDestroy()
    {
        // Désabonnement propre
        if (_startZones != null)
        {
            foreach (var zone in _startZones)
            {
                if (zone != null)
                {
                    zone.OnChaseBegin -= OnChaseStarted;
                }
            }
        }

        if (_endZones != null)
        {
            foreach (var zone in _endZones)
            {
                if (zone != null)
                {
                    zone.OnChaseEnd -= OnChaseEnded;
                }
            }
        }
    }

    /// <summary>
    /// Update method
    /// </summary>
    private void Update()
    {
        // Si on est en train de faire un fade out, on continue de mettre à jour
        if (_isFadingOut)
        {
            UpdateCurrentState();

            // Vérifier si tous les volumes sont à 0
            if (AreAllVolumesZero())
            {
                Debug.Log("✅ Fade out terminé, arrêt des AudioSources");
                _isFadingOut = false;
                StopAllAudioSources();
            }
            return;
        }

        // Si la chase n'est pas active, ne rien faire
        if (!_isChaseActive) return;

        if (_distanceCalculators == null || _distanceCalculators.Length == 0) return;

        // Trouver la distance du monstre le plus proche
        _monsterDistance = GetClosestMonsterDistance();

        ChaseState newState = EvaluateStateFromDistance(_monsterDistance);

        if (newState != _currentState)
        {
            TransitionToState(newState);
        }

        UpdateCurrentState();
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Appelé quand le joueur entre dans une zone de START
    /// </summary>
    private void OnChaseStarted()
    {
        Debug.Log("🏃 CHASE DÉMARRÉE!");

        _isChaseActive = true;
        _isFadingOut = false;

        // Démarrer la musique
        _heartbeat.Play();
        _percussion.Play();
        _stringAccent.Play();
        _stringMelody.Play();
        _tuba.Play();
    }

    /// <summary>
    /// Appelé quand le joueur entre dans une zone de END
    /// </summary>
    private void OnChaseEnded()
    {
        Debug.Log("🛑 CHASE TERMINÉE! Début du fade out...");

        _isChaseActive = false;
        _isFadingOut = true; // Activer le fade out

        // Forcer l'état NO_CHASE pour déclencher le fade out
        _previousState = _currentState;
        _currentState = ChaseState.NO_CHASE;
    }
    #endregion

    #region Private Methods - Setup
    /// <summary>
    /// Configure les AudioSources (état initial)
    /// </summary>
    private void SetupAudioSources()
    {
        _heartbeat.volume = 0;
        _heartbeat.loop = true;

        _percussion.volume = 0;
        _percussion.loop = true;

        _stringAccent.volume = 0;
        _stringAccent.loop = true;

        _stringMelody.volume = 0;
        _stringMelody.loop = true;

        _tuba.volume = 0;
        _tuba.loop = true;

        Debug.Log("🎵 AudioSources configurées");
    }

    /// <summary>
    /// Vérifie si tous les volumes sont à 0
    /// </summary>
    private bool AreAllVolumesZero()
    {
        float threshold = 0.01f; // Seuil de tolérance

        return _heartbeat.volume < threshold &&
               _percussion.volume < threshold &&
               _stringAccent.volume < threshold &&
               _stringMelody.volume < threshold &&
               _tuba.volume < threshold;
    }

    /// <summary>
    /// Arrête toutes les AudioSources et réinitialise
    /// </summary>
    private void StopAllAudioSources()
    {
        _heartbeat.Stop();
        _percussion.Stop();
        _stringAccent.Stop();
        _stringMelody.Stop();
        _tuba.Stop();

        // Remettre les volumes exactement à 0
        _heartbeat.volume = 0;
        _percussion.volume = 0;
        _stringAccent.volume = 0;
        _stringMelody.volume = 0;
        _tuba.volume = 0;

        // Remettre le temps de lecture à 0
        _heartbeat.time = 0;
        _percussion.time = 0;
        _stringAccent.time = 0;
        _stringMelody.time = 0;
        _tuba.time = 0;

        Debug.Log("🔇 Toutes les AudioSources arrêtées et réinitialisées");
    }
    #endregion

    #region Private Methods - Distance Calculation
    /// <summary>
    /// Retourne la distance du monstre le plus proche
    /// </summary>
    private float GetClosestMonsterDistance()
    {
        float minDistance = float.MaxValue;

        foreach (var calculator in _distanceCalculators)
        {
            if (calculator != null)
            {
                float distance = calculator.Distance;
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }

        // Si aucun monstre n'est trouvé, retourner une distance très grande
        return minDistance == float.MaxValue ? _distanceForNoChase + 1 : minDistance;
    }
    #endregion

    #region Private Methods - State Management
    private ChaseState EvaluateStateFromDistance(float distance)
    {
        if (distance >= _distanceForNoChase) return ChaseState.NO_CHASE;
        if (distance >= _distanceForFar) return ChaseState.FAR;
        if (distance >= _distanceForMid) return ChaseState.MID;
        if (distance >= _distanceForNear) return ChaseState.NEAR;
        if (distance >= _distanceForVeryClose) return ChaseState.VERY_CLOSE;
        return ChaseState.VERY_CLOSE;
    }

    private void TransitionToState(ChaseState newState)
    {
        _previousState = _currentState;
        _currentState = newState;

        Debug.Log($"🎵 Chase state: {_previousState} → {_currentState}");

        switch (newState)
        {
            case ChaseState.NO_CHASE:
                //FadeOut Brutal
                break;
            case ChaseState.FAR:
                break;
            case ChaseState.MID:
                break;
            case ChaseState.NEAR:
                break;
            case ChaseState.VERY_CLOSE:
                break;
        }
    }

    private void UpdateCurrentState()
    {
        float volumeSpeed = 1.4f / _fadeTime;

        switch (_currentState)
        {
            case ChaseState.NO_CHASE:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, 0f, volumeSpeed * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, 0f, volumeSpeed * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, 0f, volumeSpeed * Time.deltaTime);
                break;

            case ChaseState.FAR:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeFar, volumeSpeed * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, 0f, volumeSpeed * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, 0f, volumeSpeed * Time.deltaTime);
                break;

            case ChaseState.MID:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeMid, volumeSpeed * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, _volumeMid, volumeSpeed * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, 0f, volumeSpeed * Time.deltaTime);
                break;

            case ChaseState.NEAR:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeNear, volumeSpeed * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, volumeSpeed * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, _volumeNear, volumeSpeed * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, _volumeNear, volumeSpeed * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, _volumeNear, volumeSpeed * Time.deltaTime);
                break;

            case ChaseState.VERY_CLOSE:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeVeryClose, volumeSpeed * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, _volumeNear * 2 / 3, volumeSpeed * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, _volumeVeryClose, volumeSpeed * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, _volumeVeryClose, volumeSpeed * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, _volumeVeryClose, volumeSpeed * Time.deltaTime);
                break;
        }
    }
    #endregion
}

public enum ChaseState
{
    NO_CHASE,
    FAR,
    MID,
    NEAR,
    VERY_CLOSE
}