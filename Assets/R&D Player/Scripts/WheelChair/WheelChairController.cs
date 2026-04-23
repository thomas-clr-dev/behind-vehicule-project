using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Obligatoire pour le nouveau système

public class WheelChairController : MonoBehaviour
{
    [Header("Control Scheme")]
    [Tooltip("Select the control scheme for the wheel. This will determine how the wheel responds to player input.")]
    [SerializeField] private ControlScheme wheelMode;

    [Space(10)] 
    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider RightWheelCollider;
    [SerializeField] public WheelCollider LeftWheelCollider;
    [SerializeField] private WheelCollider rightFrontWheelCollider;
    [SerializeField] private WheelCollider leftFrontWheelCollider;

    [Space(10)]
    [Header("Wheel Visuals")]
    [SerializeField] private Transform rightWheel;
    [SerializeField] private Transform leftWheel;
    [SerializeField] private Transform rightFrontWheel;
    [SerializeField] private Transform leftFrontWheel;

    [Space(10)]
    [Header("Physics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;

    [Space(10)]
    [Header("Motor Settings")]
    [SerializeField] public float MotorTorque = 150f;
    [SerializeField] private AnimationCurve torqueCurve;

    [Space(10)]
    [Header("Sync Settings")]
    [SerializeField] private float syncWindow = 0.1f; // Temps max entre deux poussées (100ms)
    private float lastPushTime;
    private HandType lastHandToPush;

    [Space(10)]
    [Header("Animation Rigging")]
    [SerializeField] private WheelPushController leftVisual;
    [SerializeField] private WheelPushController rightVisual;
    public AnimationCurve TorqueCurve => torqueCurve;

    private Vector3 position;
    private Quaternion rotation;

    private RDPlayerActions controls;

    private StateMachine leftHandMachine;
    private StateMachine rightHandMachine;



    private void Awake()
    {
        controls = new RDPlayerActions();

        leftHandMachine = new StateMachine();
        rightHandMachine = new StateMachine();

        WheelStateContext leftHandcontext = new WheelStateContext(wheelMode, this, controls, HandType.LeftHand);
        WheelStateContext rightHandContext = new WheelStateContext(wheelMode, this, controls, HandType.RightHand);

        leftHandMachine.Initialize(wheelMode.CreateState(leftHandcontext));
        rightHandMachine.Initialize(wheelMode.CreateState(rightHandContext));
    }

    // Activer/Désactiver les actions
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        if (rb == null) return;
        rb.solverIterations = 12;
        rb.solverVelocityIterations = 12;

        if (centerOfMass != null)
            rb.centerOfMass = centerOfMass.localPosition;
    }

    private void Update()
    {
        UpdateWheelVisuals();

        leftHandMachine?.UpdateState();
        rightHandMachine?.UpdateState();
    }

    private void FixedUpdate()
    {
        leftHandMachine?.PhysicsUpdateState();
        rightHandMachine?.PhysicsUpdateState();
    }

    private void UpdateWheelVisuals()
    {
        UpdateSingleWheel(LeftWheelCollider, leftWheel);
        UpdateSingleWheel(RightWheelCollider, rightWheel);
        UpdateSingleWheel(leftFrontWheelCollider, leftFrontWheel);
        UpdateSingleWheel(rightFrontWheelCollider, rightFrontWheel);
    }

    private void UpdateSingleWheel(WheelCollider collider, Transform visualTransform)
    {
        if (collider == null || visualTransform == null) return;
        collider.GetWorldPose(out position, out rotation);
        visualTransform.position = position;
        visualTransform.rotation = rotation;
    }

    public void UpdateHandVisual(HandType hand, float progress)
    {
        if (hand == HandType.LeftHand && leftVisual != null) leftVisual.SetVisualProgress(progress);
        if (hand == HandType.RightHand && rightVisual != null) rightVisual.SetVisualProgress(progress);
    }

}