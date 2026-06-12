using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ferme automatiquement une porte par rotation quand le joueur entre dans le trigger
/// </summary>
public class AutoCloseDoor : MonoBehaviour
{
    #region SerializeField
    [Header("Door Settings")]
    [Tooltip("La porte qui va tourner (peut être cet objet ou un enfant)")]
    [SerializeField] private Transform _doorTransform;

    [Tooltip("Angle de rotation pour fermer la porte (degrés)")]
    [SerializeField] private float _closedAngle = 90f;

    [Tooltip("Axe de rotation")]
    [SerializeField] private RotationAxis _rotationAxis = RotationAxis.Y;

    [Tooltip("Sens de rotation (1 = positif, -1 = négatif)")]
    [SerializeField] private float _rotationDirection = 1f;

    [Header("Animation Settings")]
    [Tooltip("Durée de la fermeture (secondes)")]
    [SerializeField] private float _closeDuration = 1.5f;

    [Tooltip("Courbe d'animation (ease in/out)")]
    [SerializeField] private AnimationCurve _closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Trigger Settings")]
    [Tooltip("Fermer seulement une fois")]
    [SerializeField] private bool _closeOnce = true;

    [Tooltip("Délai avant de fermer (secondes)")]
    [SerializeField] private float _closeDelay = 0.5f;

    [Header("Audio (Optional)")]
    [Tooltip("Son de fermeture de porte")]
    [SerializeField] private List<AudioClip> _closeSoundList;
    [SerializeField] private AudioSource _audioSource;

    [Header("Gizmos")]
    [Tooltip("Afficher les gizmos de visualisation")]
    [SerializeField] private bool _showGizmos = true;

    [Tooltip("Taille de la flèche de direction")]
    [SerializeField] private float _arrowSize = 1f;

    [Header("Debug")]
    [Tooltip("Afficher les logs de debug")]
    [SerializeField] private bool _showDebugLogs = true;

    #endregion

    #region Private Fields
    private bool _hasClosed = false;
    private Quaternion _initialRotation;
    private Quaternion _targetRotation;
    private bool _isClosing = false;
    #endregion
    

    #region Initialization
    private void Start()
    {
        // Si la porte n'est pas assignée, utiliser cet objet
        if (_doorTransform == null)
        {
            _doorTransform = transform;
        }

        // Sauvegarder la rotation initiale
        _initialRotation = _doorTransform.localRotation;

        // Calculer la rotation cible
        CalculateTargetRotation();

        // Récupérer l'AudioSource si non assigné
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_showDebugLogs)
        {
            //Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Initialisée. Angle: {_closedAngle}°, Axe: {_rotationAxis}, Direction: {(_rotationDirection > 0 ? "+" : "-")}");
        }
    }

    /// <summary>
    /// Calcule la rotation cible en fonction de l'axe et de la direction
    /// </summary>
    private void CalculateTargetRotation()
    {
        Vector3 rotationEuler = _initialRotation.eulerAngles;
        float angle = _closedAngle * _rotationDirection;

        switch (_rotationAxis)
        {
            case RotationAxis.X:
                rotationEuler.x += angle;
                break;
            case RotationAxis.Y:
                rotationEuler.y += angle;
                break;
            case RotationAxis.Z:
                rotationEuler.z += angle;
                break;
        }

        _targetRotation = Quaternion.Euler(rotationEuler);
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_closeOnce && _hasClosed)
            {
                if (_showDebugLogs)
                {
                    Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Déjà fermée, ignoré");
                }
                return;
            }

            if (_isClosing)
            {
                if (_showDebugLogs)
                {
                    Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Déjà en train de se fermer");
                }
                return;
            }

            if (_showDebugLogs)
            {
                Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Joueur détecté, fermeture dans {_closeDelay}s");
            }

            _hasClosed = true;

            // Lancer la fermeture avec délai
            if (_closeDelay > 0f)
            {
                StartCoroutine(CloseDoorAfterDelay());
            }
            else
            {
                StartCoroutine(CloseDoor());
            }
        }
    }
    #endregion

    #region Door Animation
    /// <summary>
    /// Attend le délai puis ferme la porte
    /// </summary>
    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(_closeDelay);

        int r = Random.Range(0, _closeSoundList.Count);
        _audioSource.PlayOneShot(_closeSoundList[r], 1f);

        StartCoroutine(CloseDoor());
    }

    /// <summary>
    /// Anime la fermeture de la porte
    /// </summary>
    private IEnumerator CloseDoor()
    {
        _isClosing = true;

        if (_showDebugLogs)
        {
            Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Début de la fermeture");
        }

        // Jouer le son
        if (_audioSource != null && _closeSoundList != null)
        {
            //_audioSource.PlayOneShot(_closeSound);

        }

        float elapsed = 0f;

        while (elapsed < _closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _closeDuration;

            // Appliquer la courbe d'animation
            float curveValue = _closeCurve.Evaluate(t);

            // Interpoler entre la rotation initiale et la rotation cible
            _doorTransform.localRotation = Quaternion.Lerp(_initialRotation, _targetRotation, curveValue);

            yield return null;
        }

        // S'assurer que la rotation finale est exacte
        _doorTransform.localRotation = _targetRotation;

        _isClosing = false;

        if (_showDebugLogs)
        {
            Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Fermeture terminée");

        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Réinitialise la porte à sa position initiale
    /// </summary>
    public void ResetDoor()
    {
        StopAllCoroutines();
        _doorTransform.localRotation = _initialRotation;
        _hasClosed = false;
        _isClosing = false;

        if (_showDebugLogs)
        {
            Debug.Log($"🚪 AutoCloseDoor '{gameObject.name}' - Réinitialisée");
        }
    }

    /// <summary>
    /// Force la fermeture immédiate
    /// </summary>
    public void CloseImmediately()
    {
        StopAllCoroutines();
        _doorTransform.localRotation = _targetRotation;
        _hasClosed = true;
        _isClosing = false;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // ✅ Dessiner la zone de trigger (BoxCollider)
        DrawTriggerZone();

        // ✅ Dessiner la direction de rotation de la porte
        if (_doorTransform != null)
        {
            DrawRotationArrow();
        }
    }

    /// <summary>
    /// Dessine la zone de trigger du BoxCollider
    /// </summary>
    private void DrawTriggerZone()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            // Couleur selon l'état
            Color fillColor;
            Color wireColor;

            if (_hasClosed)
            {
                fillColor = new Color(1f, 0f, 0f, 0.2f); // Rouge semi-transparent
                wireColor = new Color(1f, 0f, 0f, 0.8f); // Rouge
            }
            else if (_isClosing)
            {
                fillColor = new Color(1f, 0.5f, 0f, 0.3f); // Orange semi-transparent
                wireColor = new Color(1f, 0.5f, 0f, 1f); // Orange
            }
            else
            {
                fillColor = new Color(0f, 1f, 0f, 0.2f); // Vert semi-transparent
                wireColor = new Color(0f, 1f, 0f, 0.8f); // Vert
            }

            // Dessiner le cube rempli
            Gizmos.color = fillColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);

            // Dessiner le contour du cube
            Gizmos.color = wireColor;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

            // ✅ Dessiner les coins du BoxCollider pour mieux voir les limites
            DrawBoxCorners(boxCollider, wireColor);
        }
        else
        {
            // Avertissement si pas de BoxCollider
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position, "⚠️ Missing BoxCollider!");
            #endif
        }
    }

    /// <summary>
    /// Dessine les coins du BoxCollider pour mieux visualiser
    /// </summary>
    private void DrawBoxCorners(BoxCollider box, Color color)
    {
        Gizmos.color = color;
        Vector3 size = box.size;
        Vector3 center = box.center;

        // Les 8 coins du cube (en local space)
        Vector3[] corners = new Vector3[8]
        {
            center + new Vector3(-size.x, -size.y, -size.z) * 0.5f,
            center + new Vector3( size.x, -size.y, -size.z) * 0.5f,
            center + new Vector3( size.x, -size.y,  size.z) * 0.5f,
            center + new Vector3(-size.x, -size.y,  size.z) * 0.5f,
            center + new Vector3(-size.x,  size.y, -size.z) * 0.5f,
            center + new Vector3( size.x,  size.y, -size.z) * 0.5f,
            center + new Vector3( size.x,  size.y,  size.z) * 0.5f,
            center + new Vector3(-size.x,  size.y,  size.z) * 0.5f
        };

        // Dessiner des petites sphères aux coins
        float cornerSize = Mathf.Min(size.x, size.y, size.z) * 0.05f;
        foreach (Vector3 corner in corners)
        {
            Gizmos.DrawSphere(corner, cornerSize);
        }
    }

    

    /// <summary>
    /// Dessine une flèche indiquant la direction de rotation
    /// </summary>
    private void DrawRotationArrow()
    {
        Vector3 doorPosition = _doorTransform.position;
        Vector3 axisDirection = Vector3.zero;
        Vector3 rotationPlaneNormal = Vector3.zero;

        // Déterminer l'axe de rotation
        switch (_rotationAxis)
        {
            case RotationAxis.X:
                axisDirection = _doorTransform.right;
                rotationPlaneNormal = _doorTransform.up;
                break;
            case RotationAxis.Y:
                axisDirection = _doorTransform.up;
                rotationPlaneNormal = _doorTransform.forward;
                break;
            case RotationAxis.Z:
                axisDirection = _doorTransform.forward;
                rotationPlaneNormal = _doorTransform.right;
                break;
        }

        // ✅ Dessiner l'axe de rotation (ligne jaune épaisse)
        Gizmos.color = Color.yellow;
        Vector3 axisStart = doorPosition - axisDirection * _arrowSize * 0.6f;
        Vector3 axisEnd = doorPosition + axisDirection * _arrowSize * 0.6f;
        DrawThickLine(axisStart, axisEnd, 0.05f);

        // ✅ Dessiner un arc indiquant la direction de rotation
        Color arcColor = _rotationDirection > 0 ? Color.cyan : Color.magenta;
        DrawRotationArc(doorPosition, axisDirection, rotationPlaneNormal, arcColor);

        // ✅ Dessiner une flèche indiquant la direction finale
        DrawDirectionArrow(doorPosition, axisDirection, rotationPlaneNormal, arcColor);

        // ✅ Dessiner le label de l'angle
        DrawAngleLabel(doorPosition, axisDirection);
    }

    /// <summary>
    /// Dessine une ligne épaisse
    /// </summary>
    private void DrawThickLine(Vector3 start, Vector3 end, float thickness)
    {
        #if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.DrawAAPolyLine(thickness * 10f, start, end);
        #else
        Gizmos.DrawLine(start, end);
        #endif
    }

    /// <summary>
    /// Dessine le label de l'angle de rotation
    /// </summary>
    private void DrawAngleLabel(Vector3 center, Vector3 axis)
    {
        #if UNITY_EDITOR
        Vector3 labelPos = center + axis * (_arrowSize * 0.7f);
        string angleText = $"{Mathf.Abs(_closedAngle)}°";
        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        
        UnityEditor.Handles.Label(labelPos, angleText, style);
        #endif
    }

    /// <summary>
    /// Dessine un arc pour visualiser la rotation
    /// </summary>
    private void DrawRotationArc(Vector3 center, Vector3 axis, Vector3 planeNormal, Color color)
    {
        Gizmos.color = color;

        int segments = 30;
        float angleStep = (_closedAngle * _rotationDirection) / segments;
        Vector3 previousPoint = center + planeNormal * _arrowSize;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 newPoint = center + rotation * (planeNormal * _arrowSize);

            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawAAPolyLine(3f, previousPoint, newPoint);
            #else
            Gizmos.DrawLine(previousPoint, newPoint);
            #endif

            previousPoint = newPoint;
        }
    }

    /// <summary>
    /// Dessine une flèche à la fin de l'arc
    /// </summary>
    private void DrawDirectionArrow(Vector3 center, Vector3 axis, Vector3 planeNormal, Color color)
    {
        Gizmos.color = color;

        // Point final de l'arc
        Quaternion finalRotation = Quaternion.AngleAxis(_closedAngle * _rotationDirection, axis);
        Vector3 endPoint = center + finalRotation * (planeNormal * _arrowSize);

        // Dessiner une sphère au point final
        Gizmos.DrawSphere(endPoint, _arrowSize * 0.08f);

        // Calculer la direction de la flèche
        Vector3 tangent = Vector3.Cross(axis, finalRotation * planeNormal).normalized * _rotationDirection;
        
        // Dessiner les pointes de la flèche
        float arrowHeadLength = _arrowSize * 0.25f;
        Vector3 arrowHead1 = endPoint - tangent * arrowHeadLength + axis * arrowHeadLength * 0.3f;
        Vector3 arrowHead2 = endPoint - tangent * arrowHeadLength - axis * arrowHeadLength * 0.3f;

        #if UNITY_EDITOR
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawAAPolyLine(4f, endPoint, arrowHead1);
        UnityEditor.Handles.DrawAAPolyLine(4f, endPoint, arrowHead2);
        #else
        Gizmos.DrawLine(endPoint, arrowHead1);
        Gizmos.DrawLine(endPoint, arrowHead2);
        #endif

        // Dessiner un cercle au point de départ
        Gizmos.color = Color.white;
        Vector3 startPoint = center + planeNormal * _arrowSize;
        Gizmos.DrawWireSphere(startPoint, _arrowSize * 0.08f);
    }
    #endregion
}

/// <summary>
/// Axe de rotation de la porte
/// </summary>
public enum RotationAxis
{
    X,
    Y,
    Z
}
