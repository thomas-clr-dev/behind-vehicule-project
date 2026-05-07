using UnityEngine;

public struct PushEvent 
{
    public HandType HandType;
    public PushEvent(HandType handType)
    {
        HandType = handType;
    }

    static PushEvent e;
    public static void Trigger(HandType handType)
    {
        e.HandType = handType;
        EventBus.TriggerEvent(e);
    }
}
