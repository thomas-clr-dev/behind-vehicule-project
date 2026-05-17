using UnityEngine;

public abstract class ControlScheme : ScriptableObject
{
    public float MotorTorque = 150f;
    public abstract IState CreateState(WheelStateContext context);
}
