using UnityEngine;

[CreateAssetMenu(fileName = "ArcadeMode", menuName = "ControlSchemes/ArcadeMode", order = 2)]
public class ArcadeMode : ControlScheme
{
    public AnimationCurve TorqueCurve;
    public override IState CreateState(WheelStateContext context)
    {
        return new ArcadeWheelState(context);
    }
}
