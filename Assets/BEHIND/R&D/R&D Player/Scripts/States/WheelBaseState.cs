
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


    protected virtual float GetMoveInput() => handType == HandType.LeftHand ? controls.Player.MoveLeft.ReadValue<float>() : controls.Player.MoveRight.ReadValue<float>();
    protected virtual float GetPushInput() => handType == HandType.LeftHand ? controls.Player.PushLeft.ReadValue<float>() : controls.Player.PushRight.ReadValue<float>();
    protected virtual WheelCollider GetWheel() => handType == HandType.LeftHand ? hub.LeftWheelCollider : hub.RightWheelCollider;

    protected virtual void  HandlePushPressed()
    {
        isPushPressed = controls.Player.PushLeft.WasPressedThisFrame() || controls.Player.PushRight.WasPressedThisFrame();

        if(isPushPressed)
        {
           //EventBus<PushEvent>.Raise(new PushEvent(handType));
            PushEvent.Trigger(handType);
        }
    }
}