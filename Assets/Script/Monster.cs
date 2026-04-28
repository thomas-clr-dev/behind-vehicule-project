using UnityEngine;

public class Monster : MonoBehaviour
{
    #region SerializeField
    [Header("Chase Settings")]
    [Tooltip("Vitesse de déplacement du monstre en X")]
    [SerializeField] private float _chaseSpeed = 4f;
    #endregion

    #region Private Fields
    private bool _isChasing = false;
    [SerializeField] private ChaseTriggerZone _chaseTriggerZone;
    #endregion

    #region Initialization
    private void Start()
    {
        // Essayer GetComponentInChildren
        //_chaseTriggerZone = GetComponentInChildren<ChaseTriggerZone>();

        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin += StartChasing;
        }
        else
        {

            // Essayer de la trouver dans toute la scène
            _chaseTriggerZone = FindObjectOfType<ChaseTriggerZone>();
            if (_chaseTriggerZone != null)
            {
                _chaseTriggerZone.OnChaseBegin += StartChasing;
            }
        }
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_isChasing)
        {
            transform.position += new Vector3(_chaseSpeed * Time.deltaTime, 0, 0);
        }
    }
    #endregion

    #region Chase Logic
    private void StartChasing()
    {
        _isChasing = true;
    }

    public void StopChasing()
    {
        _isChasing = false;
    }
    #endregion

    #region Debug (À RETIRER APRÈS)
    private void OnDestroy()
    {
        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin -= StartChasing;
        }
    }
    #endregion
}
