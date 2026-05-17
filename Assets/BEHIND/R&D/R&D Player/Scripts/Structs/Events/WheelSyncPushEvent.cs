public struct WheelSyncPushEvent
{
    public float Duration;
    public float Direction;
    public HandType Initiator;

    public WheelSyncPushEvent(float duration, float direction, HandType initiator)
    {
        Duration = duration;
        Direction = direction;
        Initiator = initiator;
    }

   
    static WheelSyncPushEvent e;
    public static void Trigger(float duration, float direction, HandType initiator)
    {
        e.Duration = duration;
        e.Direction = direction;
        e.Initiator = initiator;
        EventBus.Publish(e);
    }
}