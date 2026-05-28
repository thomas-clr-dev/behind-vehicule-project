using UnityEngine;

public class IKHipsModule : MonoBehaviour
{
    [SerializeField] private Transform hipsTransform;
    [SerializeField] private Transform target;

    // Offset calculé une fois, appliqué en continu
    private Vector3 _positionOffset;
    private Quaternion _rotationOffset;

    void Start()
    {
        // On calcule l'écart entre la position actuelle des hanches et la cible
        _positionOffset = target.position - hipsTransform.position;
        _rotationOffset = Quaternion.Inverse(hipsTransform.rotation) * target.rotation;
    }

    // LateUpdate s'exécute APRÈS l'Animator → on écrase sa position correctement
    void LateUpdate()
    {
        hipsTransform.position = target.position;
        // Si tu veux aussi corriger la rotation (utile pour la pose assise) :
        hipsTransform.rotation = target.rotation;
    }
}