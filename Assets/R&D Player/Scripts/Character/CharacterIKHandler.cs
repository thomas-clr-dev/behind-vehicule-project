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
        float visualProg = 0.5f;

        if (e.Step == GestureStep.Pushing || e.Step == GestureStep.Cooldown)
        {
            visualProg = Mathf.InverseLerp(0.3f * -e.PushDirection, 0.8f * e.PushDirection, e.StickY);
        }

        // Application au composant visuel
        var visual = (e.Hand == HandType.LeftHand) ? leftHandVisual : rightHandVisual;
        if (visual != null) visual.SetVisualProgress(visualProg);
    }

    private void OnDisable()
    {
       EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}