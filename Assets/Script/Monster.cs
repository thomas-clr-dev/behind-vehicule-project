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
        _initialPosition = transform.position;

        _renderers = GetComponentsInChildren<Renderer>();

        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin += StartChasing;
        }
        else
        {
            _chaseTriggerZone = FindObjectOfType<ChaseTriggerZone>();
            if (_chaseTriggerZone != null)
            {
                _chaseTriggerZone.OnChaseBegin += StartChasing;
            }
        }

        if (_endZone != null)
        {
            if (_endZone != null)
            {
                _endZone.OnChaseEnd += StopChasing;
            }
        }
        else
        {
            _endZone = FindObjectOfType<ChaseEndZone>();
            if (_endZone != null)
            {
                _endZone.OnChaseEnd += StopChasing;
            }
        }

        if (_distanceCalculator == null)
        {
            _distanceCalculator = GetComponent<DistanceFromMonsterToPlayer>();

            if (_distanceCalculator == null)
            {
                Debug.LogWarning("⚠️ DistanceFromMonsterToPlayer non trouvé sur le monstre!");
            }
        }

        SetVisibility(false);
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_isChasing && _distanceCalculator != null)
        {
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
                case MovementAxis.NegX:
                    movement = new Vector3(-_chaseSpeed * Time.deltaTime, 0, 0);
                    break;
                case MovementAxis.NegY:
                    movement = new Vector3(0, -_chaseSpeed * Time.deltaTime, 0);
                    break;
                case MovementAxis.NegZ:
                    movement = new Vector3(0, 0, -_chaseSpeed * Time.deltaTime);
                    break;
            }

            transform.position += movement;
        }
    }
    #endregion

    #region Chase Logic
    private void StartChasing()
    {

        _isChasing = true;

        SetVisibility(true);
    }

    public void StopChasing()
    {

        _isChasing = false;

        SetVisibility(false);

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
        }
    }

    /// <summary>
    /// Réinitialise la position du monstre à sa position initiale
    /// </summary>
    private void ResetPosition()
    {
        transform.position = _initialPosition;
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
    #endregion
    }
}