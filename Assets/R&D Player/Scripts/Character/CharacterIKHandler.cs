using System;
using UnityEngine;

public class CharacterIKHandler : MonoBehaviour
{

    [Header("IK References")]
    [SerializeField] private HandIKArcVisual leftHandVisual;
    [SerializeField] private HandIKArcVisual rightHandVisual;
    [Min(1f)][SerializeField] private float handOffset;

    private EventBinding<WheelStateDataEvent> dataBinding;


    private void OnEnable()
    {
        dataBinding = new EventBinding<WheelStateDataEvent>(HandleDataUpdate);
        EventBus<WheelStateDataEvent>.Register(dataBinding);
    }

    private void HandleDataUpdate(WheelStateDataEvent e)
    {
        var visual = (e.Hand == HandType.LeftHand) ? leftHandVisual : rightHandVisual;
        if (visual == null) return;


        float visualProg = Mathf.InverseLerp(-1f, 1f, e.StickY);
        visual.SetVisualProgress(visualProg);

        if (e.Step == GestureStep.Pushing || e.Step == GestureStep.Cooldown)
        {        
            visual.SetVisualOffset(1f);
        }
        else
        {
            visual.SetVisualOffset(handOffset);
        }
    }

    private void OnDisable()
    {
       EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}