using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterIKHandler : MonoBehaviour
{

    [SerializeField] private RigBuilder rigBuilder;


    [Header("IK References")]
    [SerializeField] private HandIKArcVisual leftHandVisual;
    [SerializeField] private HandIKArcVisual rightHandVisual;
    [Min(1f)][SerializeField] private float handOffset;

    [Header("Final IK Rig Targets")]
    [SerializeField] private Transform leftArmEffector;
    [SerializeField] private Transform rightArmEffector;

    [Header("Free Hand Targets (Points dans le vide)")]
    [SerializeField] private Transform leftFreeHandTarget;
    [SerializeField] private Transform rightFreeHandTarget;

    [Header("Grip Tuning (Ajustements)")]
    [Tooltip("Décalage de la main par rapport à la cible (Local)")]
    [SerializeField] private Vector3 gripOffset;
    [Tooltip("Rotation supplémentaire pour bien aligner la paume")]
    [SerializeField] private Vector3 gripRotationOffset;

    [Header("Base Rotations")]
    [SerializeField] private Vector3 leftBaseRotation;
    [SerializeField] private Vector3 rightBaseRotation;

    [SerializeField] private float transitionSpeed = 5f;

    private float leftWeight = 1f;
    private float rightWeight = 1f;

    private float leftTargetWeight = 1f;
    private float rightTargetWeight = 1f;

    private float leftPushDir = 1f;
    private float rightPushDir = 1f;

    private EventBinding<WheelStateDataEvent> dataBinding;


    private void OnEnable()
    {
        dataBinding = new EventBinding<WheelStateDataEvent>(HandleDataUpdate);
        EventBus<WheelStateDataEvent>.Register(dataBinding);
    }


    private void Update()
    {
        leftWeight = Mathf.MoveTowards(leftWeight, leftTargetWeight, Time.deltaTime * transitionSpeed);
        rightWeight = Mathf.MoveTowards(rightWeight, rightTargetWeight, Time.deltaTime * transitionSpeed);

        // On ajoute un booléen "isLeft" à la fin pour identifier la main
        UpdateEffector(leftArmEffector, leftHandVisual.ikTarget, leftFreeHandTarget, leftWeight, leftPushDir, true);
        UpdateEffector(rightArmEffector, rightHandVisual.ikTarget, rightFreeHandTarget, rightWeight, rightPushDir, false);

        if (rigBuilder != null) rigBuilder.Build();
    }

    private void UpdateEffector(Transform effector, Transform wheelPoint, Transform freePoint, float weight, float direction, bool isLeft)
    {
        if (effector == null) return;

        Vector3 finalFreePos = freePoint.position;
        if (direction < 0)
        {
            Vector3 offset = freePoint.position - wheelPoint.position;
            finalFreePos = wheelPoint.position - offset;
        }
        effector.position = Vector3.Lerp(finalFreePos, wheelPoint.position, weight);

        Vector3 rotation = isLeft ? leftBaseRotation : rightBaseRotation;
        effector.localRotation = Quaternion.Euler(rotation);

        if (weight > 0.01f)
        {
            Vector3 adjustedGripOffset = gripOffset;
            Vector3 adjustedRotationOffset = gripRotationOffset;

            if (isLeft)
            {
                adjustedGripOffset.x *= -1;
                adjustedRotationOffset.y *= -1;
                adjustedRotationOffset.z *= -1;
            }

            effector.Translate(adjustedGripOffset * weight, Space.Self);
            effector.Rotate(adjustedRotationOffset * weight, Space.Self);
        }
    }

    private void HandleDataUpdate(WheelStateDataEvent e)
    {
        var visual = (e.Hand == HandType.LeftHand) ? leftHandVisual : rightHandVisual;

        if (visual == null) return;

        visual.SetVisualProgress(Mathf.InverseLerp(-1f, 1f, e.StickY));

        float weightGoal = (e.Step == GestureStep.Cooldown) ? 0f : 1f;

        if (e.Hand == HandType.LeftHand)
        {
            leftTargetWeight = weightGoal;
            leftPushDir = e.PushDirection; // On enregistre la direction !
        }
        else
        {
            rightTargetWeight = weightGoal;
            rightPushDir = e.PushDirection;
        }
    }

    private void OnDisable()
    {
       EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}