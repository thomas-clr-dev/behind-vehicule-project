using UnityEngine;

public class ArcadeWheelState : WheelStateBase
{
    private float chrono; // Chrono propre à cette main

    private ArcadeMode data;

    public ArcadeWheelState(WheelStateContext context) : base(context)
    {
        data = context.data as ArcadeMode;
    }

    public override void Update()
    {
        float input = GetMoveInput();
        bool isPushing = GetPushInput() > 0.1f;
        WheelCollider wheel = GetWheel();

        if (isPushing && Mathf.Abs(input) > 0.1f)
        {
            // On fait défiler le chrono
            chrono += Time.deltaTime;
            if (chrono >= 1f) chrono = 0f;

            // On récupère la direction
            float direction = input > 0 ? 1f : -1f;

            // On applique le couple en utilisant la courbe du hub
            wheel.motorTorque = data.MotorTorque * direction * data.TorqueCurve.Evaluate(chrono);
            wheel.brakeTorque = 0;
        }
        else
        {
            // Reset quand on relâche
            chrono = 0;
            wheel.motorTorque = 0;
            wheel.brakeTorque = 50f;
        }
    }
}