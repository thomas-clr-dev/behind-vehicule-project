using UnityEngine;


public enum PlayerEventTypes { PlayerSpawn }
public struct PlayerEvent 
{
    public PlayerEventTypes EventType;
    public WheelChairController TargetCharacter;

    public PlayerEvent(PlayerEventTypes eventType, WheelChairController targetCharacter)
    {
        EventType = eventType;
        TargetCharacter = targetCharacter;
    }

    static PlayerEvent e;
    public static void Trigger(PlayerEventTypes eventType, WheelChairController targetCharacter)
    {
        e.EventType = eventType;
        e.TargetCharacter = targetCharacter;
        EventBus.Publish(e);
    }
}
