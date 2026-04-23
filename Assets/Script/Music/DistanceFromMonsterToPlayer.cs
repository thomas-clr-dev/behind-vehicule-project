using Unity.VisualScripting;
using UnityEngine;

public class DistanceFromMonsterToPlayer : MonoBehaviour
{
    #region References
    [SerializeField] private GameObject _player;
    #endregion

    #region Manual Distance Override
    [Header("Manual Testing")]
    [Tooltip("Activer pour tester manuellement la distance avec le slider")]
    [SerializeField] private bool _useManualDistance = true;

    [Tooltip("Distance manuelle (pour tester le système audio)")]
    [Range(0, 30)]
    [SerializeField] private int _manualDistance = -22;
    #endregion

    #region Distance
    private float _distance;
    public float Distance => _distance;
    #endregion

    #region Distance Calculation
    private void Update()
    {
        if (_useManualDistance)
        {
            _distance = _manualDistance;
            transform.position = new Vector3(_manualDistance, transform.position.y, transform.position.z);
        }
        else
        {
            if (_player == null) return;
            _distance = Vector3.Distance(transform.position, _player.transform.position);
        }

        Debug.Log($"Distance: {_distance} {(_useManualDistance ? "(MANUAL)" : "(AUTO)")}");
    }
    #endregion

    #region OnDrawGizmos
    private void OnDrawGizmos()
    {
        if (_player == null) return;

        // Couleur différente selon le mode
        Gizmos.color = _useManualDistance ? Color.yellow : Color.red;
        Gizmos.DrawLine(transform.position, _player.transform.position);
    }
    #endregion
}
