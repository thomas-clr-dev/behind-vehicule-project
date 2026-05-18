public struct WheelStateDataEvent
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

    // Logique TDE
    static WheelStateDataEvent e;
    public static void Trigger(HandType hand, GestureStep step, float stickY, float pushDir, float motorTorque)
    {
        e.Hand = hand;
        e.Step = step;
        e.StickY = stickY;
        e.PushDirection = pushDir;
        e.MotorTorque = motorTorque;
        EventBus.Publish(e);
    }
}