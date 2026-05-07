using UnityEngine;
using UnityEngine.XR;

public class IKHandModule : MonoBehaviour, EventListener<WheelStateDataEvent>
{
    [Header("Side Settings")]
    [SerializeField] private HandType side; 
    [Header("References")]
    [SerializeField] private HandIKArcVisual handVisual;
    [SerializeField] private Transform armEffector;
    [SerializeField] private Transform freeHandTarget;

    [Header("Rotations Settings")]
    [SerializeField] private Vector3 backRotation;
    [SerializeField] private Vector3 forwardRotation;
    [SerializeField] private Vector3 baseRotation;

    [Header("Offsets")]
    [SerializeField] private Vector3 gripOffset;
    [SerializeField] private Vector3 gripRotationOffset;

    [Header("Speeds")]
    [SerializeField] private float transitionSpeed = 5f;

    private float currentWeight = 1f;
    private float targetWeight = 1f;
    private float pushDir = 1f;

    private void OnEnable()
    {
        this.EventStartListening<WheelStateDataEvent>();
    }

    private void HandleUpdate(WheelStateDataEvent e)
    {
        if (e.Hand != side) return;

        handVisual.SetVisualProgress(Mathf.InverseLerp(-1f, 1f, e.StickY));
        targetWeight = (e.Step == GestureStep.Cooldown) ? 0f : 1f;
        pushDir = e.PushDirection;
    }

    private void Update()
    {
        currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);

        // --- Calcul Position ---
        Vector3 finalFreePos = freeHandTarget.position;
        if (pushDir < 0)
        {
            Vector3 offset = freeHandTarget.position - handVisual.ikTarget.position;
            finalFreePos = handVisual.ikTarget.position - offset;
        }
        armEffector.position = Vector3.Lerp(finalFreePos, handVisual.ikTarget.position, currentWeight);

        // --- Calcul Rotation ---
        Vector3 targetFreeEuler = (pushDir >= 0) ? forwardRotation : backRotation;
        Quaternion freeRot = Quaternion.Euler(targetFreeEuler);
        Quaternion wheelRot = Quaternion.Euler(baseRotation);
        armEffector.localRotation = Quaternion.Slerp(freeRot, wheelRot, currentWeight);

        // --- Application Offsets ---
        if (currentWeight > 0.01f)
        {
            Vector3 adjGrip = gripOffset;
            Vector3 adjRot = gripRotationOffset;
            if (side == HandType.LeftHand) { adjGrip.x *= -1; adjRot.y *= -1; adjRot.z *= -1; }

            armEffector.Translate(adjGrip * currentWeight, Space.Self);
            armEffector.Rotate(adjRot * currentWeight, Space.Self);
        }
    }

    private void OnDisable() => this.EventStopListening<WheelStateDataEvent>();

    public void OnEvent(WheelStateDataEvent eventType)
    {
       HandleUpdate(eventType);
    }
}