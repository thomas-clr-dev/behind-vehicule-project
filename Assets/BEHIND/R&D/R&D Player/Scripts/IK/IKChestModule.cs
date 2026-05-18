using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class IKChestModule : MonoBehaviour, IEventListener<WheelStateDataEvent>
{
    [SerializeField] private MultiAimConstraint chestConstraint;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private Tween weightTween;

    private void OnEnable()
    {
        this.EventStartListening<WheelStateDataEvent>();
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
        this.EventStopListening< WheelStateDataEvent>();
    }

    public void OnEvent(WheelStateDataEvent e)
    {
        float targetWeight = 0f;

        if (e.Step == GestureStep.Cooldown && e.PushDirection > 0)
        {
            targetWeight = 1f;
        }

        ApplyWeight(targetWeight);
    }
}