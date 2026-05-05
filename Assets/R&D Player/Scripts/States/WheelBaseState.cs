
using UnityEngine;

public enum HandType
{
    LeftHand,
    RightHand
}


public abstract class WheelStateBase : IState
{
    protected WheelChairController hub;
    protected HandType handType;
    protected RDPlayerActions controls;

    private bool isPushPressed;

    public WheelStateBase(WheelStateContext context)
    {
        this.hub = context.hub;
        this.controls = context.controls;
        this.handType = context.handType;
    }

    public virtual void Enter() { }
    public virtual void Update() 
    { 
        HandlePushPressed();
    }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }


    protected virtual float GetMoveInput() => handType == HandType.LeftHand ? controls.Wheel.MoveLeft.ReadValue<float>() : controls.Wheel.MoveRight.ReadValue<float>();
    protected virtual float GetPushInput() => handType == HandType.LeftHand ? controls.Wheel.PushLeft.ReadValue<float>() : controls.Wheel.PushRight.ReadValue<float>();
    protected virtual WheelCollider GetWheel() => handType == HandType.LeftHand ? hub.LeftWheelCollider : hub.RightWheelCollider;

    protected virtual void  HandlePushPressed()
    {
        isPushPressed = controls.Wheel.PushLeft.WasPressedThisFrame() || controls.Wheel.PushRight.WasPressedThisFrame();

        if(isPushPressed)
        {
           EventBus<PushEvent>.Raise(new PushEvent(handType));
        }
    }
}