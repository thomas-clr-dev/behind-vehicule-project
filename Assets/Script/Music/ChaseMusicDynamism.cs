using UnityEngine;

public class ChaseMusicDynamism : MonoBehaviour
{
    #region SerializeField - Distance
    /// <summary>
    /// Reference to the distance calculator of the monster
    /// </summary>
    [SerializeField] private DistanceFromMonsterToPlayer _distanceCalculator;
    [SerializeField] private int _distanceForNoChase = 20;
    [SerializeField] private int _distanceForFar = 15;
    [SerializeField] private int _distanceForMid = 10;
    [SerializeField] private int _distanceForNear = 5;
    [SerializeField] private int _distanceForVeryClose = 0;
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
    [Header("Audio Settings - Pitch")]
    [Tooltip("Pitch quand FAR")]
    [SerializeField] private float _pitchFar = 0.8f;
    [Tooltip("Pitch quand MID")]
    [SerializeField] private float _pitchMid = 0.9f;
    [Tooltip("Pitch quand NEAR")]
    [SerializeField] private float _pitchNear = 1f;
    [Tooltip("Pitch quand VERY CLOSE")]
    [SerializeField] private float _pitchVeryClose = 1.1f;

    [Header("Audio Settings - Volume")]
    [Tooltip("Volume quand FAR")]
    [SerializeField] private float _volumeFar = 0.8f;
    [Tooltip("Volume quand MID")]
    [SerializeField] private float _volumeMid = 1f;
    [Tooltip("Volume quand NEAR")]
    [SerializeField] private float _volumeNear = 1.2f;
    [Tooltip("Volume quand VERY CLOSE")]
    [SerializeField] private float _volumeVeryClose = 1.4f;
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
    /// Actual pitch for music
    /// </summary>
    float _currentPitch = 1f;
    #endregion


    #region Update
    /// <summary>
    /// Start Method
    /// </summary>
    private void Start()
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

        _heartbeat.Play();
        _percussion.Play();
        _stringAccent.Play();
        _stringMelody.Play();
        _tuba.Play();
    }

    /// <summary>
    /// Update method
    /// </summary>
    private void Update()
    {
        if (_distanceCalculator == null) return;

        _monsterDistance = _distanceCalculator.Distance;

        ChaseState newState = EvaluateStateFromDistance(_monsterDistance);

        if (newState != _currentState)
        {
            TransitionToState(newState);
        }

        UpdateCurrentState();

    }
    #endregion

    #region Private Methods
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
        float targetPitch = 1f;
        float targetVolume = 0f;

        switch (_currentState)
        {
            case ChaseState.NO_CHASE:
                targetPitch = 1f;
                targetVolume = 0f;
                
                _heartbeat.volume = Mathf.Lerp(_heartbeat.volume, targetVolume, Time.deltaTime * 2f);
                _percussion.volume = Mathf.Lerp(_percussion.volume, targetVolume, Time.deltaTime * 2f);
                _stringAccent.volume = Mathf.Lerp(_stringAccent.volume, targetVolume, Time.deltaTime * 2f);
                _stringMelody.volume = Mathf.Lerp(_stringMelody.volume, targetVolume, Time.deltaTime * 2f);
                _tuba.volume = Mathf.Lerp(_tuba.volume, targetVolume, Time.deltaTime * 2f);
                break;

            case ChaseState.FAR:
                targetPitch = _pitchFar;
                
                _heartbeat.volume = Mathf.Lerp(_heartbeat.volume, _volumeFar, Time.deltaTime * 2f);
                _percussion.volume = Mathf.Lerp(_percussion.volume, 0, Time.deltaTime * 2f);
                _stringAccent.volume = Mathf.Lerp(_stringAccent.volume, _volumeFar, Time.deltaTime * 2f);
                _stringMelody.volume = Mathf.Lerp(_stringMelody.volume, _volumeFar, Time.deltaTime * 2f);
                _tuba.volume = Mathf.Lerp(_tuba.volume, _volumeFar, Time.deltaTime * 2f);
                break;

            case ChaseState.MID:
                targetPitch = _pitchMid;
                
                _heartbeat.volume = Mathf.Lerp(_heartbeat.volume, _volumeMid, Time.deltaTime * 2f);
                _percussion.volume = Mathf.Lerp(_percussion.volume, 0, Time.deltaTime * 2f);
                _stringAccent.volume = Mathf.Lerp(_stringAccent.volume, _volumeMid, Time.deltaTime * 2f);
                _stringMelody.volume = Mathf.Lerp(_stringMelody.volume, _volumeMid, Time.deltaTime * 2f);
                _tuba.volume = Mathf.Lerp(_tuba.volume, _volumeMid, Time.deltaTime * 2f);
                break;

            case ChaseState.NEAR:
                targetPitch = _pitchNear;
                
                _heartbeat.volume = Mathf.Lerp(_heartbeat.volume, _volumeNear, Time.deltaTime * 2f);
                _percussion.volume = Mathf.Lerp(_percussion.volume, _volumeNear / 2, Time.deltaTime * 2f);
                _stringAccent.volume = Mathf.Lerp(_stringAccent.volume, _volumeNear, Time.deltaTime * 2f);
                _stringMelody.volume = Mathf.Lerp(_stringMelody.volume, _volumeNear, Time.deltaTime * 2f);
                _tuba.volume = Mathf.Lerp(_tuba.volume, _volumeNear, Time.deltaTime * 2f);
                break;

            case ChaseState.VERY_CLOSE:
                targetPitch = _pitchVeryClose;
                
                _heartbeat.volume = Mathf.Lerp(_heartbeat.volume, _volumeVeryClose, Time.deltaTime * 2f);
                _percussion.volume = Mathf.Lerp(_percussion.volume, _volumeVeryClose, Time.deltaTime * 2f);
                _stringAccent.volume = Mathf.Lerp(_stringAccent.volume, _volumeVeryClose, Time.deltaTime * 2f);
                _stringMelody.volume = Mathf.Lerp(_stringMelody.volume, _volumeVeryClose, Time.deltaTime * 2f);
                _tuba.volume = Mathf.Lerp(_tuba.volume, _volumeVeryClose, Time.deltaTime * 2f);
                break;
        }

        _currentPitch = Mathf.Lerp(_currentPitch, targetPitch, Time.deltaTime * 2f);

        _heartbeat.pitch = targetPitch;
        _percussion.pitch = targetPitch;
        _stringAccent.pitch = targetPitch;
        _stringMelody.pitch = targetPitch;
        _tuba.pitch = targetPitch;
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
