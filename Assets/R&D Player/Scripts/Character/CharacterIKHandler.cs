using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterIKHandler : MonoBehaviour
{
    [Header("IK References")]

    [Tooltip("Visuals pour les arcs de mouvement des mains")]
    [SerializeField] private HandIKArcVisual leftHandVisual;
    [Tooltip("Visuals pour les arcs de mouvement des mains")]
    [SerializeField] private HandIKArcVisual rightHandVisual;

    [Space(10)]
    [Header("Final IK Rig Targets")]

    [Tooltip("Effector final pour les bras, cible finale de l'IK")]
    [SerializeField] private Transform leftArmEffector;
    [Tooltip("Effector final pour les bras, cible finale de l'IK")]
    [SerializeField] private Transform rightArmEffector;

    [Space(10)]
    [Header("Free Hand Targets (Points dans le vide)")]

    [Tooltip("Points de référence pour les mains quand elles ne sont pas sur la roue")]
    [SerializeField] private Transform leftFreeHandTarget;
    [Tooltip("Points de référence pour les mains quand elles ne sont pas sur la roue")]
    [SerializeField] private Transform rightFreeHandTarget;

    [Space(10)]
    [Header("Free Hand Rotation(Back)")]

    [Tooltip("Rotation des mains quand elles ne sont pas sur la roue")]
    [SerializeField] private Vector3 backLeftFreeHandRotationTarget;
    [Tooltip("Rotation des mains quand elles ne sont pas sur la roue")]
    [SerializeField] private Vector3 backRightFreeHandRotationTarget;

    [Space(10)]
    [Header("Free Hand Rotation(Front)")]

    [Tooltip("Rotation des mains quand elles ne sont pas sur la roue")]
    [SerializeField] private Vector3 forwardLeftFreeHandRotationTarget;
    [Tooltip("Rotation des mains quand elles ne sont pas sur la roue")]
    [SerializeField] private Vector3 forwardRightFreeHandRotationTarget;

    [Space(10)]
    [Header("Grip Tuning (Ajustements)")]

    [Tooltip("Distance de la main par rapport à la cible (Local)")]
    [SerializeField] private Vector3 gripOffset;
    [Tooltip("Rotation supplémentaire pour bien aligner la paume")]
    [SerializeField] private Vector3 gripRotationOffset;

    [Space(10)]
    [Header("Base Rotations")]

    [Tooltip("Rotation de base pour les bras quand ils sont sur la roue (Local)")]
    [SerializeField] private Vector3 leftBaseRotation;
    [Tooltip("Rotation de base pour les bras quand ils sont sur la roue (Local)")]
    [SerializeField] private Vector3 rightBaseRotation;

    [Space(10)]
    [Header("Chest Rigging")]

    [Tooltip("Effector pour la poitrine, utilisé pour ajouter du mouvement de torse")]
    [SerializeField] private MultiAimConstraint chestConstraint;
    [SerializeField] private float chestTransitionSpeed = 3f;

    private float chestTargetWeight = 0f;

    [Space(10)]
    [Header("Transition Settings")]

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

        UpdateHandEffector(leftArmEffector, leftHandVisual.ikTarget, leftFreeHandTarget, leftWeight, leftPushDir, true);
        UpdateHandEffector(rightArmEffector, rightHandVisual.ikTarget, rightFreeHandTarget, rightWeight, rightPushDir, false);

        UpdateChestEffector();

    }

    private void UpdateChestEffector()
    {
        if (chestConstraint == null) return;

        // On fait bouger le poids réel vers le poids cible
        chestConstraint.weight = Mathf.MoveTowards(
            chestConstraint.weight,
            chestTargetWeight,
            Time.deltaTime * chestTransitionSpeed
        );
    }

    private void UpdateHandEffector(Transform effector, Transform wheelPoint, Transform freePoint, float weight, float direction, bool isLeft)
    {
        if (effector == null) return;

        Vector3 finalFreePos = freePoint.position;
        if (direction < 0)
        {
            Vector3 offset = freePoint.position - wheelPoint.position;
            finalFreePos = wheelPoint.position - offset;
        }
        effector.position = Vector3.Lerp(finalFreePos, wheelPoint.position, weight);

        Vector3 targetFreeRotationEuler;
        if (direction >= 0)
        {
            targetFreeRotationEuler = isLeft ? forwardLeftFreeHandRotationTarget : forwardRightFreeHandRotationTarget;
        }
        else
        {
            targetFreeRotationEuler = isLeft ? backLeftFreeHandRotationTarget : backRightFreeHandRotationTarget;
        }

        Vector3 baseRotationEuler = isLeft ? leftBaseRotation : rightBaseRotation;

        Quaternion freeRot = Quaternion.Euler(targetFreeRotationEuler);
        Quaternion wheelRot = Quaternion.Euler(baseRotationEuler);

        effector.localRotation = Quaternion.Slerp(freeRot, wheelRot, weight);

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

        chestTargetWeight = (e.Step == GestureStep.Cooldown) ? 1f : 0f;

        if (e.Hand == HandType.LeftHand)
        {
            leftTargetWeight = weightGoal;
            leftPushDir = e.PushDirection; 
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