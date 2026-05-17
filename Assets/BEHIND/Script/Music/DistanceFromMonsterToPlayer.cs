using Unity.VisualScripting;
using UnityEngine;
using System.Collections; // ✅ Ajouter pour les Coroutines

public class DistanceFromMonsterToPlayer : MonoBehaviour
{
    #region References
    [Header("Player Reference (Optional)")]
    [Tooltip("Référence au joueur (si vide, recherche automatique de 'PlayerCollider' avec tag 'Player')")]
    [SerializeField] private GameObject _player;

    [Header("Auto-Find Settings")]
    [Tooltip("Délai avant la première recherche (secondes)")]
    [SerializeField] private float _initialSearchDelay = 0.5f;

    [Tooltip("Nombre maximum de tentatives")]
    [SerializeField] private int _maxRetries = 10;

    [Tooltip("Délai entre chaque tentative (secondes)")]
    [SerializeField] private float _retryInterval = 0.5f;
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

    #region Initialization
    private void Start()
    {
        if (_player == null)
        {
            StartCoroutine(FindPlayerWithRetry());
        }
    }

    /// <summary>
    /// Coroutine qui cherche le player avec plusieurs tentatives
    /// </summary>
    private IEnumerator FindPlayerWithRetry()
    {
        yield return new WaitForSeconds(_initialSearchDelay);

        int attempts = 0;

        while (_player == null && attempts < _maxRetries)
        {
            attempts++;

            FindPlayerCollider();

            if (_player != null)
            {
                yield break;
            }

            yield return new WaitForSeconds(_retryInterval);
        }

        if (_player == null)
        {
            Debug.LogError($"❌ DistanceFromMonsterToPlayer '{gameObject.name}' - Player introuvable après {_maxRetries} tentatives!");
        }
    }

    /// <summary>
    /// Cherche automatiquement l'objet "PlayerCollider" avec le tag "Player"
    /// </summary>
    private void FindPlayerCollider()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            return;
        }

        foreach (GameObject player in players)
        {
            if (player.name == "PlayerCollider")
            {
                _player = player;
                return;
            }
        }

        foreach (GameObject player in players)
        {
            if (player.name.Contains("PlayerCollider"))
            {
                _player = player;
                return;
            }
        }
    }
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