using UnityEngine;

public class ChaseMusicDynamism : MonoBehaviour
{
    #region Distance

    [InspectorGroup("Distance Settings", true, 20)]
    [SerializeField] private DistanceFromMonsterToPlayer[] _distanceCalculators;
    [SerializeField] private int _distanceForNoChase = 20;
    [SerializeField] private int _distanceForFar = 15;
    [SerializeField] private int _distanceForMid = 10;
    [SerializeField] private int _distanceForNear = 5;
    [SerializeField] private int _distanceForVeryClose = 0;

    #endregion

    #region Chase Triggers

    [InspectorGroup("Chase Triggers", true, 23)]
    [Tooltip("Zone(s) de déclenchement de la chase")]
    [SerializeField] private ChaseTriggerZone[] _startZones;

    [Tooltip("Zone(s) de fin de la chase")]
    [SerializeField] private ChaseEndZone[] _endZones;

    [Tooltip("Zone de victoire")]
    [SerializeField] private VictoryTriggerZone _victoryZone;

    #endregion

    #region Audio Layers

    [InspectorGroup("Audio Layers", true, 14)]
    [SerializeField] private AudioSource _heartbeat;
    [SerializeField] private AudioSource _percussion;
    [SerializeField] private AudioSource _stringAccent;
    [SerializeField] private AudioSource _stringMelody;
    [SerializeField] private AudioSource _tuba;

    #endregion

    #region Audio Settings

    [InspectorGroup("Audio Settings", true, 39)]
    [Tooltip("Volume quand FAR")]
    [SerializeField] private float _volumeFar = 0.8f;
    [Tooltip("Volume quand MID")]
    [SerializeField] private float _volumeMid = 1f;
    [Tooltip("Volume quand NEAR")]
    [SerializeField] private float _volumeNear = 1.2f;
    [Tooltip("Volume quand VERY CLOSE")]
    [SerializeField] private float _volumeVeryClose = 1.4f;

    [InspectorGroup("Fade Time", true, 51)]
    [Tooltip("Durée de la transition linéaire en secondes")]
    [SerializeField] private float _fadeTime = 3f;

    #endregion

    #region Private Fields

    private float _monsterDistance;
    private ChaseState _currentState;
    private ChaseState _previousState;
    private bool _isChaseActive = false;
    private bool _isFadingOut = false;
    private bool _isGameEnded = false;

    #endregion

    #region Unity Methods

    private void Start()
    {
        SetupAudioSources();

        Monster.OnGameOver += OnGameEnded;

        if (_victoryZone != null)
            _victoryZone.OnVictory += OnGameEnded;

        if (_startZones != null && _startZones.Length > 0)
            foreach (var zone in _startZones)
                if (zone != null) zone.OnChaseBegin += OnChaseStarted;

        if (_endZones != null && _endZones.Length > 0)
            foreach (var zone in _endZones)
                if (zone != null) zone.OnChaseEnd += OnChaseEnded;
    }

    private void OnDestroy()
    {
        Monster.OnGameOver -= OnGameEnded;

        if (_victoryZone != null)
            _victoryZone.OnVictory -= OnGameEnded;

        if (_startZones != null)
            foreach (var zone in _startZones)
                if (zone != null) zone.OnChaseBegin -= OnChaseStarted;

        if (_endZones != null)
            foreach (var zone in _endZones)
                if (zone != null) zone.OnChaseEnd -= OnChaseEnded;
    }

    private void Update()
    {
        if (_isGameEnded) return;

        if (_isFadingOut)
        {
            UpdateCurrentState();
            if (AreAllVolumesZero()) { _isFadingOut = false; StopAllAudioSources(); }
            return;
        }

        if (!_isChaseActive) return;
        if (_distanceCalculators == null || _distanceCalculators.Length == 0) return;

        _monsterDistance = GetClosestMonsterDistance();

        ChaseState newState = EvaluateStateFromDistance(_monsterDistance);
        if (newState != _currentState) TransitionToState(newState);

        UpdateCurrentState();
    }

    #endregion

    #region Event Handlers

    private void OnChaseStarted()
    {
        if (_isGameEnded) return;
        _isChaseActive = true;
        _isFadingOut = false;
        _heartbeat.Play();
        _percussion.Play();
        _stringAccent.Play();
        _stringMelody.Play();
        _tuba.Play();
    }

    private void OnChaseEnded()
    {
        if (_isGameEnded) return;
        _isChaseActive = false;
        _isFadingOut = true;
        _previousState = _currentState;
        _currentState = ChaseState.NO_CHASE;
    }

    private void OnGameEnded()
    {
        _isGameEnded = true;
        _isChaseActive = false;
        _isFadingOut = false;
        StopAllAudioSources();
    }

    #endregion

    #region Setup

    private void SetupAudioSources()
    {
        _heartbeat.volume = 0; _heartbeat.loop = true;
        _percussion.volume = 0; _percussion.loop = true;
        _stringAccent.volume = 0; _stringAccent.loop = true;
        _stringMelody.volume = 0; _stringMelody.loop = true;
        _tuba.volume = 0; _tuba.loop = true;
    }

    private bool AreAllVolumesZero()
    {
        float t = 0.01f;
        return _heartbeat.volume < t && _percussion.volume < t &&
               _stringAccent.volume < t && _stringMelody.volume < t && _tuba.volume < t;
    }

    private void StopAllAudioSources()
    {
        _heartbeat.Stop(); _heartbeat.volume = 0; _heartbeat.time = 0;
        _percussion.Stop(); _percussion.volume = 0; _percussion.time = 0;
        _stringAccent.Stop(); _stringAccent.volume = 0; _stringAccent.time = 0;
        _stringMelody.Stop(); _stringMelody.volume = 0; _stringMelody.time = 0;
        _tuba.Stop(); _tuba.volume = 0; _tuba.time = 0;
    }

    #endregion

    #region Distance Calculation

    private float GetClosestMonsterDistance()
    {
        float minDistance = float.MaxValue;
        foreach (var calculator in _distanceCalculators)
            if (calculator != null && calculator.Distance < minDistance)
                minDistance = calculator.Distance;
        return minDistance == float.MaxValue ? _distanceForNoChase + 1 : minDistance;
    }

    #endregion

    #region State Management

    private ChaseState EvaluateStateFromDistance(float distance)
    {
        if (distance >= _distanceForNoChase) return ChaseState.NO_CHASE;
        if (distance >= _distanceForFar) return ChaseState.FAR;
        if (distance >= _distanceForMid) return ChaseState.MID;
        if (distance >= _distanceForNear) return ChaseState.NEAR;
        return ChaseState.VERY_CLOSE;
    }

    private void TransitionToState(ChaseState newState)
    {
        _previousState = _currentState;
        _currentState = newState;
    }

    private void UpdateCurrentState()
    {
        float s = 1.4f / _fadeTime;

        switch (_currentState)
        {
            case ChaseState.NO_CHASE:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, 0f, s * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, s * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, 0f, s * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, 0f, s * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, 0f, s * Time.deltaTime);
                break;

            case ChaseState.FAR:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeFar, s * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, s * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, 0f, s * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, 0f, s * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, 0f, s * Time.deltaTime);
                break;

            case ChaseState.MID:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeMid, s * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, s * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, 0f, s * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, _volumeMid, s * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, 0f, s * Time.deltaTime);
                break;

            case ChaseState.NEAR:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeNear, s * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, 0f, s * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, _volumeNear, s * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, _volumeNear, s * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, _volumeNear, s * Time.deltaTime);
                break;

            case ChaseState.VERY_CLOSE:
                _heartbeat.volume = Mathf.MoveTowards(_heartbeat.volume, _volumeVeryClose, s * Time.deltaTime);
                _percussion.volume = Mathf.MoveTowards(_percussion.volume, _volumeNear * 2 / 3, s * Time.deltaTime);
                _stringAccent.volume = Mathf.MoveTowards(_stringAccent.volume, _volumeVeryClose, s * Time.deltaTime);
                _stringMelody.volume = Mathf.MoveTowards(_stringMelody.volume, _volumeVeryClose, s * Time.deltaTime);
                _tuba.volume = Mathf.MoveTowards(_tuba.volume, _volumeVeryClose, s * Time.deltaTime);
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