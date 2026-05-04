using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class IKChestModule : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint chestConstraint;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private EventBinding<WheelStateDataEvent> dataBinding;
    private Tween weightTween;

    private void OnEnable()
    {
        dataBinding = new EventBinding<WheelStateDataEvent>(e =>
        {
            float targetWeight = 0f;

            if (e.Step == GestureStep.Cooldown && e.PushDirection > 0)
            {
                targetWeight = 1f;
            }

            ApplyWeight(targetWeight);
        });
        EventBus<WheelStateDataEvent>.Register(dataBinding);
    }

    private void ApplyWeight(float target)
    {
        if (weightTween != null && chestConstraint.weight == target) return;

        weightTween?.Kill();
        weightTween = DOTween.To(() => chestConstraint.weight, x => chestConstraint.weight = x, target, duration).SetEase(easeType);
    }

    private void OnDisable()
    {
        weightTween?.Kill();
        EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }
}