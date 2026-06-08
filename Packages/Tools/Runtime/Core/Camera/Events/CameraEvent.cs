using UnityEngine;
using Tools;

public enum CameraEventType
{
    RotateTo,
    SnapTo,
}

public struct CameraEvent
{
    public CameraEventType EventType;
    public float TargetZ;
    public float Duration;
    public MyTween.TweenType TweenType;

    static CameraEvent e;

    public static void Trigger(CameraEventType eventType, float targetZ = 0f,
        float duration = 0f, MyTween.TweenType tweenType = MyTween.TweenType.EaseInOutCubic)
    {
        e.EventType = eventType;
        e.TargetZ = targetZ;
        e.Duration = duration;
        e.TweenType = tweenType;
        EventBus.Publish(e);
    }
}