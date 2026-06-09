// =============================================================================
// MyFeedbackTweenPosition.cs
// =============================================================================
// Feedback qui déplace un Transform d'un point A à un point B via le système
// MyTween. La destination et la durée peuvent être définies à l'avance dans
// l'Inspector OU surchargées dynamiquement au runtime via SetTarget().
//
// Utilisé par PawnView pour animer les déplacements et les captures.
// Le PawnView appelle SetTarget(destination, duration, tweenType) juste avant Play().
// =============================================================================

using System;
using System.Collections;
using UnityEngine;
using Tools;

[FeedbackPath("Transform/Tween Position")]
[Serializable]
public class MyFeedbackTweenPosition : MyFeedback
{
    public MyFeedbackTweenPosition()
    {
        Label = "Tween Position";
        FeedbackColor = new Color(0.2f, 0.8f, 0.4f); // Vert
    }

    [InspectorGroup("Target", true, 36)]
    [Tooltip("Le Transform à déplacer. Si null, utilise le Transform de l'Owner.")]
    public Transform TargetTransform;

    [InspectorGroup("Tween Settings", true, 22)]
    [Tooltip("Courbe d'interpolation utilisée pour le mouvement.")]
    public MyTween.TweenType TweenCurve = MyTween.TweenType.EaseOutCubic;

    [Tooltip("Position de destination. Peut être surchargée au runtime via SetTarget().")]
    public Vector3 DestinationPosition;

    // État interne
    [NonSerialized] private Coroutine _tweenCoroutine;

    /// <summary>
    /// Surcharge dynamique de la destination, durée et courbe au runtime.
    /// Appelé par PawnView juste avant Play() pour passer les paramètres de l'event.
    /// </summary>
    public void SetTarget(Vector3 destination, float duration, MyTween.TweenType curve)
    {
        DestinationPosition = destination;
        Duration = duration;
        TweenCurve = curve;
    }

    /// <summary>
    /// Surcharge simplifiée : juste la destination avec la durée et courbe par défaut.
    /// </summary>
    public void SetTarget(Vector3 destination)
    {
        DestinationPosition = destination;
    }

    protected override void CustomPlay()
    {
        Transform target = TargetTransform != null ? TargetTransform : Owner.transform;
        Vector3 origin = target.position;

        _tweenCoroutine = Owner.StartCoroutine(TweenPositionCo(target, origin, DestinationPosition, Duration, TweenCurve));
    }

    protected override void CustomStop()
    {
        if (_tweenCoroutine != null)
        {
            Owner.StopCoroutine(_tweenCoroutine);
            _tweenCoroutine = null;
        }

        // Snap à la destination finale
        Transform target = TargetTransform != null ? TargetTransform : Owner.transform;
        target.position = DestinationPosition;
    }

    protected override void CustomReset()
    {
        // Rien de spécial — le prochain SetTarget() redéfinira la destination
    }

    /// <summary>
    /// Coroutine d'interpolation utilisant MyTween.Tween pour calculer la position
    /// à chaque frame. S'arrête quand la durée est écoulée et snap à la destination.
    /// </summary>
    private IEnumerator TweenPositionCo(Transform target, Vector3 origin, Vector3 destination, float duration, MyTween.TweenType curve)
    {
        if (duration <= 0f)
        {
            target.position = destination;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.position = MyTween.Tween(elapsed, 0f, duration, origin, destination, curve);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.position = destination; // Snap final pour éviter les imprécisions
        _tweenCoroutine = null;
    }
}