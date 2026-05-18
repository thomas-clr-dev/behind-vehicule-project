using UnityEngine;

public class HandIKArcVisual : MonoBehaviour
{
    public enum Axis { X, Y, Z, Forward, Right, Up }

    [Header("Références")]
    public Transform pivot;
    public Transform ikTarget;

    [Header("Réglages des Axes")]
    public Axis rotationAxis = Axis.X;
    public Axis radiusDirection = Axis.Up;

    [Header("Réglages de l'Arc")]
    [Range(-180f, 180f)] public float angleMin = -45f;
    [Range(-180f, 180f)] public float angleMax = 45f;
    public float radius = 0.35f;

    [Header("Animation & Smoothing")]
    [Range(0f, 1f)] public float targetProgress = 0f; // La cible voulue
    public float smoothSpeed = 10f; // Vitesse de lissage

    private float currentProgress = 0f; // La valeur lissée actuelle

    [Header("Debug")]
    public bool debug = true;

    void Update()
    {
        UpdateHandPosition();
    }

    private void UpdateHandPosition()
    {
        if (pivot == null || ikTarget == null) return;

        currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * smoothSpeed);

        float currentAngle = Mathf.Lerp(angleMin, angleMax, currentProgress);

        Vector3 axisVector = GetAxisVector(rotationAxis);
        pivot.localRotation = Quaternion.AngleAxis(currentAngle, axisVector);

        Vector3 direction = GetAxisVector(radiusDirection);
        ikTarget.position = pivot.TransformPoint(direction * radius);
    }

    Vector3 GetAxisVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.X: case Axis.Right: return Vector3.right;
            case Axis.Y: case Axis.Up: return Vector3.up;
            case Axis.Z: case Axis.Forward: return Vector3.forward;
            default: return Vector3.up;
        }
    }

    // Appeler cette méthode pour changer la position de la main
    public void SetVisualProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (pivot == null || !debug) return;

        UnityEditor.Handles.matrix = pivot.parent != null ? pivot.parent.localToWorldMatrix : Matrix4x4.identity;

        Vector3 center = pivot.localPosition;
        Vector3 rotAxis = GetAxisVector(rotationAxis);
        Vector3 radDir = GetAxisVector(radiusDirection);

        UnityEditor.Handles.color = new Color(0, 1, 0, 0.3f);
        Vector3 startDirection = Quaternion.AngleAxis(angleMin, rotAxis) * radDir;
        UnityEditor.Handles.DrawSolidArc(center, rotAxis, startDirection, angleMax - angleMin, radius);
    }
#endif
}