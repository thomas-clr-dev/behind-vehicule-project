using UnityEngine;

public struct PushEvent : IEvent
{
    public HandType HandType { get; private set; }
    public PushEvent(HandType handType)
    {
        HandType = handType;
    }
}
