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

    [Header("Animation")]
    [Range(0f, 1f)] public float pushProgress = 0f;
    public float radius = 0.35f;

    void OnValidate() { UpdateHandPosition(); }
    void Update() { UpdateHandPosition(); }

    private void UpdateHandPosition()
    {
        if (pivot == null || ikTarget == null) return;

        float currentAngle = Mathf.Lerp(angleMin, angleMax, pushProgress);

        Vector3 axisVector = GetAxisVector(rotationAxis);

        pivot.localRotation = Quaternion.AngleAxis(currentAngle, axisVector);

        ikTarget.position = pivot.position + pivot.up * radius * 0.8f;
    }

    Vector3 GetAxisVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return Vector3.right;
            case Axis.Y: return Vector3.up;
            case Axis.Z: return Vector3.forward;
            case Axis.Forward: return Vector3.forward;
            case Axis.Right: return Vector3.right;
            case Axis.Up: return Vector3.up;
            default: return Vector3.up;
        }
    }

    public void SetVisualProgress(float progress)
    {
        pushProgress = Mathf.Clamp01(progress);
        UpdateHandPosition();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (pivot == null) return;
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