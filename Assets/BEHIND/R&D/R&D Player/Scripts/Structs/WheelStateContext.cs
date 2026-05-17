using UnityEngine;

public struct WheelStateContext
{
    public WheelChairController hub;
    public RDPlayerActions controls;
    public HandType handType;
    public ControlScheme data;

    public WheelStateContext(ControlScheme data = null, WheelChairController hub = null, RDPlayerActions controls = null, HandType handType = HandType.RightHand)
    {
        this.data = data;
        this.hub = hub;
        this.controls = controls;
        this.handType = handType;
    }
}
