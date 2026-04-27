// Événement purement orienté "Données"
public struct WheelStateDataEvent : IEvent
{
    public HandType Hand;
    public GestureStep Step;
    public float StickY;
    public float PushDirection;

    public WheelStateDataEvent(HandType hand, GestureStep step, float stickY, float pushDir)
    {
        Hand = hand;
        Step = step;
        StickY = stickY;
        PushDirection = pushDir;
    }

}