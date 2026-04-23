using System;
using UnityEngine;

public enum GestureStep { WaitGrip, Pushing, Cooldown }
public class SimulationWheelState : WheelStateBase
{

    private GestureStep currentStep = GestureStep.WaitGrip;

    private float lastStickY;
    private float pushDirection = 0f;
    private SimulationMode data;

    private bool isPushing => GetPushInput() > 0.5f;
    private float stickY => GetMoveInput();

    private WheelCollider wheel;
    private float stickVelocity;

    private float pushDurationTimer = 0f;

    private HandType hand;

    public SimulationWheelState(WheelStateContext context) : base(context)
    {
        if (context.data is SimulationMode simData)
            data = simData;
        hand = context.handType;

        wheel = GetWheel();
    }

    public override void Update()
    {
        HandleState();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        float visualProg = 0f;

        if (currentStep == GestureStep.Pushing || currentStep == GestureStep.Cooldown)
        {
            visualProg = Mathf.InverseLerp(0.3f * -pushDirection, 0.8f * pushDirection, stickY);
        }
        else
        {
            visualProg = 0.5f;
        }

        hub.UpdateHandVisual(handType, visualProg);
    }


    private void SwitchState(GestureStep newState)
    {
        var lastState = currentStep;
        currentStep = newState;
        EventBus<SWheelStateChangedEvent>.Raise(new SWheelStateChangedEvent(newState, lastState));
    }

    private void HandleState()
    {
        stickVelocity = (stickY - lastStickY) / Time.deltaTime;
        lastStickY = stickY;

        switch (currentStep)
        {
            case GestureStep.WaitGrip:
                HandleWaitGrip();
                break;

            case GestureStep.Pushing:
                HandlePushing();
                break;

            case GestureStep.Cooldown:
                HandleCooldown();
                break;
        }
    }

    private void HandleWaitGrip()
    {
        wheel.motorTorque = 0;

        wheel.brakeTorque = 2f;

        if (isPushing)
        {
            // CAS 1 : On bouge le stick -> On prépare une poussée
            if (stickY < -0.3f)
            {
                pushDirection = 1f;
                SwitchState(GestureStep.Pushing);
            }
            else if (stickY > 0.3f) 
            {
                pushDirection = -1f;
                SwitchState(GestureStep.Pushing);
            }

            // CAS 2 : On agrippe sans mouvement de stick (ou stick au neutre)
            else if (Mathf.Abs(stickY) < 0.3f)
            {
                pushDurationTimer = 0f;
                SwitchState(GestureStep.Cooldown);
            }
        }
    }

    private void HandlePushing()
    {
        if (!isPushing)
        {
           SwitchState(GestureStep.WaitGrip);
            return;
        }

        bool hasCrossedThreshold = (pushDirection > 0) ? stickY > 0.1f : stickY < -0.1f;
        bool isMovingFast = (pushDirection > 0) ? stickVelocity > 1.2f : stickVelocity < -1.2f;

        if (isMovingFast && hasCrossedThreshold)
        {
            pushDurationTimer = data.pushDuration;
            SwitchState(GestureStep.Cooldown);
        }
    }

    private void HandleCooldown()
    {
        if (pushDurationTimer > 0)
        {
            wheel.motorTorque = data.MotorTorque * pushDirection;
            wheel.brakeTorque = 0;
            pushDurationTimer -= Time.deltaTime;
        }
        else
        {
            wheel.motorTorque = Mathf.MoveTowards(wheel.motorTorque, 0, data.MotorTorque * Time.deltaTime * data.MoveCutoffSpeed);

            if (isPushing)
            {

                wheel.brakeTorque = data.GripBrakeForce;
            }
            else
            {
                wheel.brakeTorque = 0;
            }

            if ((!isPushing || Mathf.Abs(stickY) < 0.2f) && Mathf.Abs(wheel.motorTorque) < 0.1f)
            {
                SwitchState(GestureStep.WaitGrip);
            }
        }
    }
}