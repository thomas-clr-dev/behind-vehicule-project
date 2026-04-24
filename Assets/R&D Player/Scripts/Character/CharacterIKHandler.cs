using System;
using UnityEngine;

public class CharacterIKHandler : MonoBehaviour
{

    [Header("IK References")]
    [SerializeField] private HandIKArcVisual leftHandVisual;
    [SerializeField] private HandIKArcVisual rightHandVisual;


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

        if (e.Step == GestureStep.Pushing || e.Step == GestureStep.Cooldown)
        {
            float visualProg = Mathf.InverseLerp(-1f, 1f, e.StickY);
            visual.SetVisualProgress(visualProg);
        }
        else
        {
            visual.SetVisualProgress(0.5f);
        }
    }

    private void OnDisable()
    {
       EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}