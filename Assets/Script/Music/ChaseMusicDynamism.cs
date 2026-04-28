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
    /// Actual pitch for music
    /// </summary>
    #endregion


    #region Unity Methods
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