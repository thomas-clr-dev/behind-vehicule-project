using UnityEngine;

public struct SWheelStateChangedEvent : IEvent
{
    public GestureStep NewState { get; }
    public GestureStep OldState { get; }

    public SWheelStateChangedEvent(GestureStep newState, GestureStep oldState)
    {
        NewState = newState;
        OldState = oldState;
    }

}
