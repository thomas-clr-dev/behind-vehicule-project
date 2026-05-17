using UnityEngine;

[CreateAssetMenu(fileName = "SimulationMode", menuName = "ControlSchemes/SimulationMode", order = 1)]
public class SimulationMode : ControlScheme
{
    public float pushDuration = 0.15f;
    public float GripBrakeForce = 50f;
    public float MoveCutoffSpeed = 10f;
    public override IState CreateState(WheelStateContext context)
    {
        return new SimulationWheelState(context);
    }
}
