// =============================================================================
// MyFeedbackFade.cs
// =============================================================================
// Feedback qui déclenche un FadeIn ou FadeOut sur un Fader via l'EventBus.
// Peut cibler un Fader précis via son ID.
// =============================================================================
using System;
using UnityEngine;
using Tools;

[FeedbackPath("UI/Fade")]
[Serializable]
public class MyFeedbackFade : MyFeedback
{
    public enum FadeDirection { In, Out, Custom }

    public MyFeedbackFade()
    {
        Label = "Fade";
        FeedbackColor = new Color(0.4f, 0.6f, 1f); // Bleu clair
        Duration = 0.3f;
    }

    [InspectorGroup("Fader", true, 10)]
    [Tooltip("ID du Fader ciblé (doit correspondre au champ ID sur le composant Fader).")]
    public int FaderID = 0;

    [InspectorGroup("Fade Settings", true, 20)]
    [Tooltip("FadeIn = apparaître, FadeOut = disparaître, Custom = alpha libre.")]
    public FadeDirection Direction = FadeDirection.In;

    [Tooltip("Alpha cible, uniquement utilisé si Direction = Custom.")]
    [Range(0f, 1f)]
    public float TargetAlpha = 1f;

    [Tooltip("Courbe d'interpolation du fade.")]
    public MyTween.TweenType TweenCurve = MyTween.TweenType.EaseInCubic;

    [Tooltip("Si vrai, le fade ignore le TimeScale (utile pour les menus pause).")]
    public bool IgnoreTimeScale = true;

    /// <summary>
    /// Surcharge dynamique au runtime — même pattern que SetTarget() dans TweenPosition.
    /// </summary>
    public void SetFade(FadeDirection direction, float duration, MyTween.TweenType curve)
    {
        Direction = direction;
        Duration = duration;
        TweenCurve = curve;
    }

    public void SetFade(FadeDirection direction)
    {
        Direction = direction;
    }

    protected override void CustomPlay()
    {
        var tweenType = new MyTweenType(TweenCurve);

        switch (Direction)
        {
            case FadeDirection.In:
                FadeInEvent.Trigger(Duration, tweenType, FaderID, IgnoreTimeScale);
                break;

            case FadeDirection.Out:
                FadeOutEvent.Trigger(Duration, tweenType, FaderID, IgnoreTimeScale);
                break;

            case FadeDirection.Custom:
                FadeEvent.Trigger(Duration, TargetAlpha, tweenType, FaderID, IgnoreTimeScale);
                break;
        }
    }

    protected override void CustomStop()
    {
        FadeStopEvent.Trigger(FaderID, restore: false);
    }

    protected override void CustomReset()
    {
        FadeStopEvent.Trigger(FaderID, restore: true);
    }
}