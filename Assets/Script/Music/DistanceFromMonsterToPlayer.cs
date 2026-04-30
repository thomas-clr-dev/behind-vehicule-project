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
    [SerializeField] private bool _useManualDistance = false;

    [Tooltip("Distance manuelle (pour tester le système audio)")]
    [Range(0, 30)]
    [SerializeField] private int _manualDistance = -22;

    [Tooltip("Axe de déplacement du monstre en mode manuel")]
    [SerializeField] private MovementAxis _movementAxis = MovementAxis.X;
    #endregion

    #region Distance
    private float _distance;
    public float Distance => _distance;

    public MovementAxis Axis => _movementAxis;
    #endregion

    #region Distance Calculation
    private void Update()
    {
        if (_useManualDistance)
        {
            _distance = _manualDistance;

            Vector3 newPosition = transform.position;

            switch (_movementAxis)
            {
                case MovementAxis.X:
                    newPosition.x = _manualDistance;
                    break;
                case MovementAxis.Y:
                    newPosition.y = _manualDistance;
                    break;
                case MovementAxis.Z:
                    newPosition.z = _manualDistance;
                    break;
                case MovementAxis.NegX:
                    newPosition.x = -_manualDistance;
                    break;
                case MovementAxis.NegY:
                    newPosition.y = -_manualDistance;
                    break;
                case MovementAxis.NegZ:
                    newPosition.z = -_manualDistance;
                    break; 
            }

            transform.position = newPosition;
        }
        else
        {
            if (_player == null) return;
            _distance = Vector3.Distance(transform.position, _player.transform.position);
        }
    }
    #endregion

    #region OnDrawGizmos
    private void OnDrawGizmos()
    {
        if (_player == null) return;

        Gizmos.color = _useManualDistance ? Color.yellow : Color.red;
        Gizmos.DrawLine(transform.position, _player.transform.position);
    }
    #endregion
}

/// <summary>
/// Axe de déplacement du monstre
/// </summary>
public enum MovementAxis
{
    X,
    Y,
    Z,
    NegX,
    NegY,
    NegZ
}