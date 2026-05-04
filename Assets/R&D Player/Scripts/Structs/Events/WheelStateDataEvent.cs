// �v�nement purement orient� "Donn�es"
public struct WheelStateDataEvent : IEvent
{
    public HandType Hand;
    public GestureStep Step;
    public float StickY;
    public float PushDirection;
    public float MotorTorque;

    public WheelStateDataEvent(HandType hand, GestureStep step, float stickY, float pushDir, float motorTorque)
    {
        Hand = hand;
        Step = step;
        StickY = stickY;
        PushDirection = pushDir;
        MotorTorque = motorTorque;
    }

}