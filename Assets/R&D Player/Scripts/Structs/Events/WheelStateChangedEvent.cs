public struct WheelStateChangedEvent
{
    public GestureStep NewState;
    public GestureStep OldState;

    public WheelStateChangedEvent(GestureStep newState, GestureStep oldState)
    {
        NewState = newState;
        OldState = oldState;
    }

    // Logique TDE
    static WheelStateChangedEvent e;
    public static void Trigger(GestureStep newState, GestureStep oldState)
    {
        e.NewState = newState;
        e.OldState = oldState;
        EventBus.TriggerEvent(e);
    }
}