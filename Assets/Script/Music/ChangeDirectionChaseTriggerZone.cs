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
    /// Le paramètre est la nouvelle direction à prendre.
    /// </summary>
    public event Action<MovementAxis> OnDirectionChange;
    #endregion

    #region SerializeField
    [Header("Direction Settings")]
    [Tooltip("Nouvelle direction que le monstre doit prendre")]
    [SerializeField] private MovementAxis _newDirection = MovementAxis.X;

    [Header("Debug")]
    [Tooltip("Afficher les logs de debug")]
    [SerializeField] private bool _showDebugLogs = true;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        // ✅ LOG : Tout ce qui entre dans la zone
        Debug.Log($"🟢 ChangeDirectionChaseTriggerZone '{gameObject.name}' - OnTriggerEnter avec: {other.gameObject.name}");

        Monster monster = other.GetComponent<Monster>();

        if (monster != null)
        {
            Debug.Log($"✅ Monster trouvé sur '{other.gameObject.name}'!");

            if (_showDebugLogs)
            {
                Debug.Log($"🔄 ChangeDirectionChaseTriggerZone '{gameObject.name}' - Monstre '{other.gameObject.name}' détecté! Nouvelle direction: {_newDirection}");
            }

            // ✅ Vérifier si l'event a des abonnés
            if (OnDirectionChange != null)
            {
                Debug.Log($"📢 Invocation de OnDirectionChange avec direction: {_newDirection}");
                OnDirectionChange.Invoke(_newDirection);
            }
            else
            {
                Debug.LogWarning($"⚠️ OnDirectionChange n'a AUCUN abonné!");
            }
        }
        else
        {
            Debug.LogWarning($"❌ Pas de Monster sur '{other.gameObject.name}'");
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);

            Vector3 arrowDirection = GetDirectionVector();
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, arrowDirection * 2f);
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
