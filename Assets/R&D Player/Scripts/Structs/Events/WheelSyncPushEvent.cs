public struct WheelSyncPushEvent : IEvent
{
    public float Duration { get; }
    public float Direction { get; }
    public HandType Initiator { get; }

    public WheelSyncPushEvent(float duration, float direction, HandType initiator)
    {
        Duration = duration;
        Direction = direction;
        Initiator = initiator;
    }
}