using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Obligatoire pour le nouveau système

[RequireComponent(typeof(Rigidbody))]
public class WheelChairController : MonoBehaviour
{
    [Header("Control Scheme")]
    [Tooltip("Select the control scheme for the wheel. This will determine how the wheel responds to player input.")]
    [SerializeField] private ControlScheme wheelMode;

    [Space(10)] 
    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider RightWheelCollider;
    [SerializeField] public WheelCollider LeftWheelCollider;
    [SerializeField] public WheelCollider RightFrontWheelCollider;
    [SerializeField] public WheelCollider LeftFrontWheelCollider;

    [Space(10)]
    [Header("Physics")]
    [SerializeField] private Transform centerOfMass;

    private Rigidbody rb;

    //LOCAl EVENTS
    public Action<HandType, float> OnHandProgressChanged;

    //INPUT SYSTEM  
    private RDPlayerActions controls;

    //STATE MACHINES    
    private StateMachine leftHandMachine; //Left Wheel
    private StateMachine rightHandMachine; // Right Wheel

    private void Awake()
    {

        rb = GetComponent<Rigidbody>();
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
        leftHandMachine?.UpdateState();
        rightHandMachine?.UpdateState();
    }

    private void FixedUpdate()
    {
        leftHandMachine?.PhysicsUpdateState();
        rightHandMachine?.PhysicsUpdateState();
    }

    public void UpdateHandVisual(HandType hand, float progress)
    {
        OnHandProgressChanged?.Invoke(hand, progress);
    }

    void OnDrawGizmos()
    {
        if (GetComponent<Rigidbody>())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(GetComponent<Rigidbody>().centerOfMass), 0.1f);
        }
    }

}