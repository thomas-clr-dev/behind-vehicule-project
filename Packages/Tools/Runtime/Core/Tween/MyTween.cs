using System.Collections;
using UnityEngine;

namespace Tools
{
    public class MyTween : MonoBehaviour
    {
        public enum TweenType
        {
            Linear,
            EaseInQuadratic, EaseOutQuadratic, EaseInOutQuadratic,
            EaseInCubic, EaseOutCubic, EaseInOutCubic,
            EaseInQuartic, EaseOutQuartic, EaseInOutQuartic,
            EaseInQuintic, EaseOutQuintic, EaseInOutQuintic,
            EaseInSinusoidal, EaseOutSinusoidal, EaseInOutSinusoidal,
            EaseInBounce, EaseOutBounce, EaseInOutBounce,
            EaseInOverhead, EaseOutOverhead, EaseInOutOverhead,
            EaseInExponential, EaseOutExponential, EaseInOutExponential,
            EaseInElastic, EaseOutElastic, EaseInOutElastic,
            EaseInCircular, EaseOutCircular, EaseInOutCircular,
            AntiLinearTween, AlmostIdentity
        }

        public delegate float TweenDelegate (float currentTime);

        public static TweenDelegate[] TweenDelegates = new TweenDelegate[]
        {
            LinearTween,
            EaseInQuadratic, EaseOutQuadratic, EaseInOutQuadratic,
            EaseInCubic, EaseOutCubic, EaseInOutCubic,
            EaseInQuartic, EaseOutQuartic, EaseInOutQuartic,
            EaseInQuintic, EaseOutQuintic, EaseInOutQuintic,
            EaseInSinusoidal, EaseOutSinusoidal, EaseInOutSinusoidal,
            EaseInBounce, EaseOutBounce, EaseInOutBounce,
            EaseInOverhead, EaseOutOverhead, EaseInOutOverhead,
            EaseInExponential, EaseOutExponential, EaseInOutExponential,
            EaseInElastic, EaseOutElastic, EaseInOutElastic,
            EaseInCircular, EaseOutCircular, EaseInOutCircular,
            AntiLinearTween, AlmostIdentity
        };


        // Core methods ---------------------------------------------------------------------------------------------------------------


        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenType curve)
        {
            currentTime = Maths.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = TweenDelegates[(int)curve](currentTime);
            return startValue + currentTime * (endValue - startValue);
        }

        public static long Tween(float currentTime, float initialTime, float endTime, long startValue, long endValue, TweenType curve)
        {
            currentTime = Maths.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = TweenDelegates[(int)curve](currentTime);
            return startValue + (long)(currentTime * (endValue - startValue));
        }

        public static float Evaluate(float t, TweenType curve)
        {
            return TweenDelegates[(int)curve](t);
        }

        public static float Evaluate(float t, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Evaluate(t, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return tweenType.Curve.Evaluate(t);
            }
            return 0f;
        }


        public static float LinearTween(float currentTime) { return TweenDefinitions.Linear_Tween(currentTime); }
        public static float AntiLinearTween(float currentTime) { return TweenDefinitions.LinearAnti_Tween(currentTime); }
        public static float EaseInQuadratic(float currentTime) { return TweenDefinitions.EaseIn_Quadratic(currentTime); }
        public static float EaseOutQuadratic(float currentTime) { return TweenDefinitions.EaseOut_Quadratic(currentTime); }
        public static float EaseInOutQuadratic(float currentTime) { return TweenDefinitions.EaseInOut_Quadratic(currentTime); }
        public static float EaseInCubic(float currentTime) { return TweenDefinitions.EaseIn_Cubic(currentTime); }
        public static float EaseOutCubic(float currentTime) { return TweenDefinitions.EaseOut_Cubic(currentTime); }
        public static float EaseInOutCubic(float currentTime) { return TweenDefinitions.EaseInOut_Cubic(currentTime); }
        public static float EaseInQuartic(float currentTime) { return TweenDefinitions  .EaseIn_Quartic(currentTime); }
        public static float EaseOutQuartic(float currentTime) { return TweenDefinitions .EaseOut_Quartic(currentTime); }
        public static float EaseInOutQuartic(float currentTime) { return TweenDefinitions.EaseInOut_Quartic(currentTime); }
        public static float EaseInQuintic(float currentTime) { return TweenDefinitions.EaseIn_Quintic(currentTime); }
        public static float EaseOutQuintic(float currentTime) { return TweenDefinitions.EaseOut_Quintic(currentTime); }
        public static float EaseInOutQuintic(float currentTime) { return TweenDefinitions.EaseInOut_Quintic(currentTime); }
        public static float EaseInSinusoidal(float currentTime) { return TweenDefinitions.EaseIn_Sinusoidal(currentTime); }
        public static float EaseOutSinusoidal(float currentTime) { return TweenDefinitions.EaseOut_Sinusoidal(currentTime); }
        public static float EaseInOutSinusoidal(float currentTime) { return TweenDefinitions.EaseInOut_Sinusoidal(currentTime); }
        public static float EaseInBounce(float currentTime) { return TweenDefinitions.EaseIn_Bounce(currentTime); }
        public static float EaseOutBounce(float currentTime) { return TweenDefinitions.EaseOut_Bounce(currentTime); }
        public static float EaseInOutBounce(float currentTime) { return TweenDefinitions.EaseInOut_Bounce(currentTime); }
        public static float EaseInOverhead(float currentTime) { return TweenDefinitions.EaseIn_Overhead(currentTime); }
        public static float EaseOutOverhead(float currentTime) { return TweenDefinitions.EaseOut_Overhead(currentTime); }
        public static float EaseInOutOverhead(float currentTime) { return TweenDefinitions.EaseInOut_Overhead(currentTime); }
        public static float EaseInExponential(float currentTime) { return TweenDefinitions.EaseIn_Exponential(currentTime); }
        public static float EaseOutExponential(float currentTime) { return TweenDefinitions.EaseOut_Exponential(currentTime); }
        public static float EaseInOutExponential(float currentTime) { return TweenDefinitions.EaseInOut_Exponential(currentTime); }
        public static float EaseInElastic(float currentTime) { return TweenDefinitions.EaseIn_Elastic(currentTime); }
        public static float EaseOutElastic(float currentTime) { return TweenDefinitions.EaseOut_Elastic(currentTime); }
        public static float EaseInOutElastic(float currentTime) { return TweenDefinitions.EaseInOut_Elastic(currentTime); }
        public static float EaseInCircular(float currentTime) { return TweenDefinitions.EaseIn_Circular(currentTime); }
        public static float EaseOutCircular(float currentTime) { return TweenDefinitions.EaseOut_Circular(currentTime); }
        public static float EaseInOutCircular(float currentTime) { return TweenDefinitions.EaseInOut_Circular(currentTime); }
        public static float AlmostIdentity(float currentTime) { return TweenDefinitions.AlmostIdentity(currentTime); }

        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenType curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenType curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Vector4 Tween(float currentTime, float initialTime, float endTime, Vector4 startValue, Vector4 endValue, TweenType curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            startValue.w = Tween(currentTime, initialTime, endTime, startValue.w, endValue.w, curve);
            return startValue;
        }

        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenType curve)
        {
            float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Animation curve methods --------------------------------------------------------------------------------------------------------------

        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
        {
            currentTime = Maths.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = curve.Evaluate(currentTime);
            return startValue + currentTime * (endValue - startValue);
        }

        public static long Tween(float currentTime, float initialTime, float endTime, long startValue, long endValue, AnimationCurve curve)
        {
            currentTime = Maths.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = curve.Evaluate(currentTime);
            return startValue + (long)currentTime * (endValue - startValue);
        }

        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Vector4 Tween(float currentTime, float initialTime, float endTime, Vector4 startValue, Vector4 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            startValue.w = Tween(currentTime, initialTime, endTime, startValue.w, endValue.w, curve);
            return startValue;
        }

        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
        {
            float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Tween type methods ------------------------------------------------------------------------------------------------------------------------

        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return 0f;
        }
        public static long Tween(float currentTime, float initialTime, float endTime, long startValue, long endValue, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return 0;
        }
        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector2.zero;
        }
        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector3.zero;
        }
        public static Vector4 Tween(float currentTime, float initialTime, float endTime, Vector4 startValue, Vector4 endValue, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector3.zero;
        }
        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, MyTweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.Tween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Quaternion.identity;
        }

        // MOVE METHODS ---------------------------------------------------------------------------------------------------------
        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Vector3 origin, Vector3 destination,
            WaitForSeconds delay, float delayDuration, float duration, MyTween.TweenType curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        public static Coroutine MoveRectTransform(MonoBehaviour mono, RectTransform targetTransform, Vector3 origin, Vector3 destination,
            WaitForSeconds delay, float delayDuration, float duration, MyTween.TweenType curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveRectTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration,
            MyTween.TweenType curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation, ignoreTimescale));
        }

        public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration,
            float duration, MyTween.TweenType curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        protected static IEnumerator MoveRectTransformCo(RectTransform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
            float delayDuration, float duration, MyTween.TweenType curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                targetTransform.localPosition = MyTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.localPosition = destination;
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
            float delayDuration, float duration, MyTween.TweenType curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                targetTransform.transform.position = MyTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination;
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration,
            MyTween.TweenType curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                if (updatePosition)
                {
                    targetTransform.transform.position = MyTween.Tween(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
                }
                if (updateRotation)
                {
                    targetTransform.transform.rotation = MyTween.Tween(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
                }
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            if (updatePosition) { targetTransform.transform.position = destination.position; }
            if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
        }

        protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration,
            MyTween.TweenType curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }

            Vector3 initialRotationPosition = targetTransform.transform.position;
            Quaternion initialRotationRotation = targetTransform.transform.rotation;

            float rate = 1f / duration;

            float timeSpent = 0f;
            while (timeSpent < duration)
            {

                float newAngle = MyTween.Tween(timeSpent, 0f, duration, 0f, angle, curve);

                targetTransform.transform.position = initialRotationPosition;
                initialRotationRotation = targetTransform.transform.rotation;
                targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
                targetTransform.transform.rotation = initialRotationRotation;

                timeSpent += ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination.position;
        }

    }
}

