using UnityEngine;

// =============================================================================
// FollowTarget
// =============================================================================
// Fait suivre ce Transform vers une cible, avec interpolation optionnelle.
// Utilisé par le SoundManager pour les sons 3D attachés à un objet mobile.
// =============================================================================
public class FollowTarget : MonoBehaviour
{
    public enum UpdateModes { Update, FixedUpdate, LateUpdate }
    public enum FollowModes { Instant, Lerp }

    [Header("Target")]
    /// Le Transform à suivre
    public Transform Target;
    /// Offset appliqué à la position cible
    public Vector3 Offset = Vector3.zero;

    [Header("Position")]
    public bool FollowPosition = true;
    public bool FollowPositionX = true;
    public bool FollowPositionY = true;
    public bool FollowPositionZ = true;

    [Header("Rotation")]
    public bool FollowRotation = false;

    [Header("Scale")]
    public bool FollowScale = false;
    public float FollowScaleFactor = 1f;

    [Header("Interpolation")]
    public FollowModes FollowMode = FollowModes.Instant;
    public float FollowPositionSpeed = 10f;
    public float FollowRotationSpeed = 10f;
    public float FollowScaleSpeed = 10f;

    [Header("Mode")]
    public UpdateModes UpdateMode = UpdateModes.Update;
    /// Si vrai, se désactive quand le GameObject est désactivé
    public bool DisableSelfOnSetActiveFalse = false;

    // -------------------------------------------------------------------------

    protected Vector3 _initialPosition;
    protected Quaternion _initialRotation;

    protected virtual void Start() => Initialization();

    public virtual void Initialization()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    public virtual void StartFollowing() => FollowPosition = true;
    public virtual void StopFollowing() => FollowPosition = false;

    // -------------------------------------------------------------------------

    protected virtual void Update()
    {
        if (UpdateMode == UpdateModes.Update) ApplyFollow();
    }

    protected virtual void FixedUpdate()
    {
        if (UpdateMode == UpdateModes.FixedUpdate) ApplyFollow();
    }

    protected virtual void LateUpdate()
    {
        if (UpdateMode == UpdateModes.LateUpdate) ApplyFollow();
    }

    protected virtual void ApplyFollow()
    {
        if (Target == null) return;

        if (FollowPosition) ApplyPosition();
        if (FollowRotation) ApplyRotation();
        if (FollowScale) ApplyScale();
    }

    protected virtual void ApplyPosition()
    {
        Vector3 targetPos = Target.position + Offset;

        if (!FollowPositionX) targetPos.x = transform.position.x;
        if (!FollowPositionY) targetPos.y = transform.position.y;
        if (!FollowPositionZ) targetPos.z = transform.position.z;

        transform.position = FollowMode == FollowModes.Lerp
            ? Vector3.Lerp(transform.position, targetPos, FollowPositionSpeed * Time.deltaTime)
            : targetPos;
    }

    protected virtual void ApplyRotation()
    {
        transform.rotation = FollowMode == FollowModes.Lerp
            ? Quaternion.Lerp(transform.rotation, Target.rotation, FollowRotationSpeed * Time.deltaTime)
            : Target.rotation;
    }

    protected virtual void ApplyScale()
    {
        Vector3 targetScale = Target.localScale * FollowScaleFactor;

        transform.localScale = FollowMode == FollowModes.Lerp
            ? Vector3.Lerp(transform.localScale, targetScale, FollowScaleSpeed * Time.deltaTime)
            : targetScale;
    }

    public virtual void ChangeTarget(Transform newTarget) => Target = newTarget;

    protected virtual void OnDisable()
    {
        if (DisableSelfOnSetActiveFalse) enabled = false;
    }
}