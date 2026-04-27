using UnityEngine;

public struct WheelStateChangedEvent : IEvent
{
    public GestureStep NewState { get; }
    public GestureStep OldState { get; }

    public WheelStateChangedEvent(GestureStep newState, GestureStep oldState)
    {
        NewState = newState;
        OldState = oldState;
    }

}
