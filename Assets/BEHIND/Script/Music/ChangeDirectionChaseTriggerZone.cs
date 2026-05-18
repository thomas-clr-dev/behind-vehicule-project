using System;
using UnityEngine;

/// <summary>
/// Zone qui change la direction du monstre quand il entre dedans
/// </summary>
public class ChangeDirectionChaseTriggerZone : MonoBehaviour
{
    #region Events
    /// <summary>
    /// Événement déclenché quand un monstre entre dans la zone.
    /// Fournit les informations de rotation et de direction.
    /// </summary>
    public event Action<DirectionChangeData> OnDirectionChange;
    #endregion

    #region SerializeField
    [Header("Direction Settings")]
    [Tooltip("Nouvelle direction que le monstre doit prendre")]
    [SerializeField] private MovementAxis _newDirection = MovementAxis.X;

    [Header("Rotation Settings")]
    [Tooltip("Angle de rotation en degrés (ex: 90, 180, 270)")]
    [SerializeField] private float _rotationAngle = 90f;

    [Tooltip("Sens de rotation")]
    [SerializeField] private RotationDirection _rotationDirection = RotationDirection.Clockwise;

    [Tooltip("Durée de la rotation en secondes")]
    [SerializeField] private float _rotationDuration = 1f;

    [Header("Speed Settings")]
    [Tooltip("Multiplicateur de vitesse pendant la rotation (0.5 = 50% de vitesse)")]
    [SerializeField] [Range(0f, 1f)] private float _speedMultiplierDuringRotation = 0.5f;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        Monster monster = other.GetComponent<Monster>();
        
        if (monster != null && OnDirectionChange != null)
        {
            DirectionChangeData changeData = new DirectionChangeData
            {
                NewDirection = _newDirection,
                RotationAngle = _rotationAngle,
                RotationDirection = _rotationDirection,
                RotationDuration = _rotationDuration,
                SpeedMultiplier = _speedMultiplierDuringRotation
            };

            OnDirectionChange.Invoke(changeData);
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            // Dessine la zone en cyan
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);

            // Dessine la direction finale en jaune
            Vector3 arrowDirection = GetDirectionVector();
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, arrowDirection * 2f);

            // Dessine un arc pour visualiser la rotation
            DrawRotationArc();
        }
    }

    /// <summary>
    /// Dessine un arc pour visualiser la rotation
    /// </summary>
    private void DrawRotationArc()
    {
        Vector3 center = transform.position;
        float radius = 1.5f;
        int segments = 20;
        
        Gizmos.color = _rotationDirection == RotationDirection.Clockwise ? Color.green : Color.magenta;

        Vector3 startDir = GetDirectionVector();
        float angleStep = _rotationAngle / segments;
        float sign = _rotationDirection == RotationDirection.Clockwise ? 1f : -1f;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * sign;
            float angle2 = (i + 1) * angleStep * sign;

            Vector3 point1 = center + Quaternion.Euler(0, angle1, 0) * startDir * radius;
            Vector3 point2 = center + Quaternion.Euler(0, angle2, 0) * startDir * radius;

            Gizmos.DrawLine(point1, point2);
        }
    }

    /// <summary>
    /// Convertit MovementAxis en Vector3 pour l'affichage
    /// </summary>
    private Vector3 GetDirectionVector()
    {
        return _newDirection switch
        {
            MovementAxis.X => Vector3.right,
            MovementAxis.Y => Vector3.up,
            MovementAxis.Z => Vector3.forward,
            MovementAxis.NegX => Vector3.left,
            MovementAxis.NegY => Vector3.down,
            MovementAxis.NegZ => Vector3.back,
            _ => Vector3.zero
        };
    }
    #endregion
}

/// <summary>
/// Sens de rotation
/// </summary>
public enum RotationDirection
{
    Clockwise,      // Sens horaire
    Anticlockwise   // Sens anti-horaire
}

/// <summary>
/// Données de changement de direction avec rotation
/// </summary>
public struct DirectionChangeData
{
    public MovementAxis NewDirection;
    public float RotationAngle;
    public RotationDirection RotationDirection;
    public float RotationDuration;
    public float SpeedMultiplier;
}
