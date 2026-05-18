
using System;
using UnityEngine;

namespace Tools
{
    public enum TweenDefinitionTypes { Tween, AnimationCurve }

    [Serializable]
    public class MyTweenType
    {
        public static MyTweenType DefaultEaseInCubic { get; } = new MyTweenType(MyTween.TweenType.EaseInCubic);
        public TweenDefinitionTypes TweenDefinitionType = TweenDefinitionTypes.Tween;
        public MyTween.TweenType TweenCurve = MyTween.TweenType.EaseInCubic;
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));
        public bool Initialized = false;

        public MyTweenType(MyTween.TweenType newCurve)
        {
            TweenCurve = newCurve;
            TweenDefinitionType = TweenDefinitionTypes.Tween;
        }
        public MyTweenType(AnimationCurve newCurve)
        {
            Curve = newCurve;
            TweenDefinitionType = TweenDefinitionTypes.AnimationCurve;
        }

        public float Evaluate(float t)
        {
            return MyTween.Evaluate(t, this);
        }
    }

}
