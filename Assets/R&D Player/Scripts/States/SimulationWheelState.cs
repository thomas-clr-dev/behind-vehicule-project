using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum GestureStep { WaitGrip, Pushing, Cooldown }
public class SimulationWheelState : WheelStateBase
{
    private GestureStep currentStep = GestureStep.WaitGrip;
    private float lastStickY;
    private float pushDirection = 0f;
    private SimulationMode data;
    private WheelCollider wheel;
    private float stickVelocity;
    private float pushDurationTimer = 0f;
    private HandType hand;

    // Propriétés calculées pour plus de clarté
    private bool isPushing => GetPushInput() > 0.5f;
    private float stickY => GetMoveInput();

    private EventBinding<WheelSyncPushEvent> dataBinding;

    public SimulationWheelState(WheelStateContext context) : base(context)
    {
        if (context.data is SimulationMode simData) data = simData;
        hand = context.handType;
        wheel = GetWheel();

        dataBinding = new EventBinding<WheelSyncPushEvent>(OnPushSynchronized);
        EventBus<WheelSyncPushEvent>.Register(dataBinding);
    }

    private void OnPushSynchronized(WheelSyncPushEvent e)
    {
        if (e.Initiator != hand)
        {
            if (currentStep == GestureStep.Cooldown || currentStep == GestureStep.Pushing)
            {
                pushDurationTimer = e.Duration;
                pushDirection = e.Direction;
                if (currentStep != GestureStep.Cooldown) SwitchState(GestureStep.Cooldown);
            }
        }
    }

    public override void Update()
    {
        stickVelocity = (stickY - lastStickY) / Time.deltaTime;
        lastStickY = stickY;

        HandleStateTransitions();

        HandleEvents();
    }

    public override void PhysicsUpdate()
    {
        ApplyWheelPhysics();
    }

    private void HandleStateTransitions()
    {
        switch (currentStep)
        {
            case GestureStep.WaitGrip:
                if (isPushing)
                {
                    if (stickY < -0.3f) { pushDirection = 1f; SwitchState(GestureStep.Pushing); }
                    else if (stickY > 0.3f) { pushDirection = -1f; SwitchState(GestureStep.Pushing); }
                }
                break;

            case GestureStep.Pushing:
                if (!isPushing) { SwitchState(GestureStep.WaitGrip); return; }

                bool hasCrossedThreshold = (pushDirection > 0) ? stickY > 0.1f : stickY < -0.1f;
                bool isMovingFast = (pushDirection > 0) ? stickVelocity > 1.2f : stickVelocity < -1.2f;

                if (isMovingFast && hasCrossedThreshold)
                {
                    pushDurationTimer = data.pushDuration;

                    EventBus<WheelSyncPushEvent>.Raise(new WheelSyncPushEvent(data.pushDuration, pushDirection, hand));
                    SwitchState(GestureStep.Cooldown);
                }
                break;

            case GestureStep.Cooldown:
                if (pushDurationTimer > 0)
                {
                    pushDurationTimer -= Time.deltaTime;
                }
                else
                {
                    if ((!isPushing || Mathf.Abs(stickY) < 0.2f) && Mathf.Abs(wheel.motorTorque) < 0.1f)
                    {
                        SwitchState(GestureStep.WaitGrip);
                    }
                }
                break;
        }
    }

    private void ApplyWheelPhysics()
    {
        switch (currentStep)
        {
            case GestureStep.WaitGrip:
                SetMotorTorque(0f);
                ApplyBrakeTorque();
                break;

            case GestureStep.Pushing:
                SetMotorTorque(0f);
                wheel.brakeTorque = 0;
                break;

            case GestureStep.Cooldown:
                if (pushDurationTimer > 0)
                {
                    float targetTorque = data.MotorTorque * pushDirection;
                    wheel.motorTorque = Mathf.MoveTowards(wheel.motorTorque, targetTorque, data.MotorTorque * Time.fixedDeltaTime * 10f);
                    wheel.brakeTorque = 0;
                }
                else
                {
                    // Freinage graduel ou saisie de la main (Grip)
                    wheel.motorTorque = Mathf.MoveTowards(wheel.motorTorque, 0, data.MotorTorque * Time.fixedDeltaTime * data.MoveCutoffSpeed);
                    ApplyBrakeTorque();
                }
                break;
        }
    }

    private void SetMotorTorque(float torque)
    {
        if( torque >= 0) wheel.motorTorque = torque;
    }

    private void ApplyBrakeTorque(float force = 0f)
    {
        wheel.brakeTorque = isPushing ? data.GripBrakeForce : 0;
    }

    private void SwitchState(GestureStep newState)
    {
        var lastState = currentStep;
        currentStep = newState;
        EventBus<WheelStateChangedEvent>.Raise(new WheelStateChangedEvent(newState, lastState));
    }

    private void HandleEvents()
    {
        EventBus<WheelStateDataEvent>.Raise(new WheelStateDataEvent(hand, currentStep, stickY, pushDirection));
    }

    public override void Exit()
    {
        EventBus<WheelSyncPushEvent>.Deregister(dataBinding);
    }
}