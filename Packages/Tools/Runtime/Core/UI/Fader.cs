using UnityEngine;
using UnityEngine.UI;
using static Tools.MyTween;

namespace Tools
{
    public struct FadeStopEvent
    {
        public int ID;
        public bool Restore;

        public FadeStopEvent(int id = 0, bool restore = false)
        {
            ID = id;
            Restore = restore;
        }

        public static void Trigger(int id = 0, bool restore = false)
        {
            EventBus.Publish(new FadeStopEvent(id, restore));
        }
    }

    public struct FadeInEvent
    {
        public int ID;
        public float Duration;
        public MyTweenType Curve;
        public bool IgnoreTimeScale;

        public FadeInEvent(float duration, MyTweenType curve, int id = 0, bool ignoreTimeScale = true)
        {
            ID = id;
            Duration = duration;
            Curve = curve;
            IgnoreTimeScale = ignoreTimeScale;
        }

        public static void Trigger(float duration, MyTweenType curve, int id = 0, bool ignoreTimeScale = true)
        {
            EventBus.Publish(new FadeInEvent(duration, curve, id, ignoreTimeScale));
        }
    }

    public struct FadeOutEvent
    {
        public int ID;
        public float Duration;
        public MyTweenType Curve;
        public bool IgnoreTimeScale;

        public FadeOutEvent(float duration, MyTweenType curve, int id = 0, bool ignoreTimeScale = true)
        {
            ID = id;
            Duration = duration;
            Curve = curve;
            IgnoreTimeScale = ignoreTimeScale;
        }

        public static void Trigger(float duration, MyTweenType curve, int id = 0, bool ignoreTimeScale = true)
        {
            EventBus.Publish(new FadeOutEvent(duration, curve, id, ignoreTimeScale));
        }
    }

    public struct FadeEvent
    {
        public int ID;
        public float Duration;
        public float TargetAlpha;
        public MyTweenType Curve;
        public bool IgnoreTimeScale;

        public FadeEvent(float duration, float targetAlpha, MyTweenType curve, int id = 0, bool ignoreTimeScale = true)
        {
            ID = id;
            Duration = duration;
            TargetAlpha = targetAlpha;
            Curve = curve;
            IgnoreTimeScale = ignoreTimeScale;
        }

        public static void Trigger(float duration, float targetAlpha, MyTweenType curve, int id = 0, bool ignoreTimeScale = true)
        {
            EventBus.Publish(new FadeEvent(duration, targetAlpha, curve, id, ignoreTimeScale));
        }
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour, IEventListener<FadeStopEvent>, IEventListener<FadeInEvent>, IEventListener<FadeOutEvent>, IEventListener<FadeEvent>
    {
        public enum ForcedInitStates { None, Active, Inactive }

        [InspectorGroup("Identification", true, 122)]
        public int ID;

        [InspectorGroup("Opacity", true, 123)]
        public float InactiveAlpha = 0f;
        public float ActiveAlpha = 1f;
        public ForcedInitStates ForcedInitState = ForcedInitStates.Inactive;

        [InspectorGroup("Timing", true, 124)]
        public float DefaultDuration = 0.2f;
        public MyTweenType DefaultTween = new MyTweenType(MyTween.TweenType.Linear);
        public bool IgnoreTimescale = true;
        public bool CanFadeToCurrentAlpha = true;

        [InspectorGroup("Interaction", true, 125)]
        public bool ShouldBlockRaycasts = false;

        protected CanvasGroup _canvasGroup;
        protected Image _image;
        protected float _initialAlpha;
        protected float _currentTargetAlpha;
        protected float _currentDuration;
        protected MyTweenType _currentCurve;
        protected bool _fading = false;
        protected float _fadeStartedAt;
        protected bool _frameCountOne;

        // -------------------------
        // Initialisation
        // -------------------------

        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _image = GetComponent<Image>();

            switch (ForcedInitState)
            {
                case ForcedInitStates.Inactive:
                    _canvasGroup.alpha = InactiveAlpha;
                    _image.enabled = false;
                    break;
                case ForcedInitStates.Active:
                    _canvasGroup.alpha = ActiveAlpha;
                    _image.enabled = true;
                    break;
            }
        }

        // -------------------------
        // Update
        // -------------------------

        protected virtual void Update()
        {
            if (_canvasGroup == null) return;
            if (_fading) Fade();
        }

        // -------------------------
        // Debug
        // -------------------------

        public void ResetFader()
        {
            _canvasGroup.alpha = InactiveAlpha;
        }

        public void DefaultFade()
        {
            FadeEvent.Trigger(DefaultDuration, ActiveAlpha, DefaultTween, ID);
        }

        // -------------------------
        // Core fade logic
        // -------------------------

        protected virtual void Fade()
        {
            float currentTime = IgnoreTimescale ? Time.unscaledTime : Time.time;

            if (_frameCountOne)
            {
                if (Time.frameCount <= 2)
                {
                    _canvasGroup.alpha = _initialAlpha;
                    return;
                }
                _fadeStartedAt = IgnoreTimescale ? Time.unscaledTime : Time.time;
                currentTime = _fadeStartedAt;
                _frameCountOne = false;
            }

            float endTime = _fadeStartedAt + _currentDuration;

            if (currentTime - _fadeStartedAt < _currentDuration)
            {
                float result = MyTween.Tween(currentTime, _fadeStartedAt, endTime, _initialAlpha, _currentTargetAlpha, _currentCurve);
                _canvasGroup.alpha = result;
            }
            else
            {
                StopFading();
            }
        }

        protected virtual void StopFading()
        {
            _canvasGroup.alpha = _currentTargetAlpha;
            _fading = false;

            if (_canvasGroup.alpha == InactiveAlpha)
                DisableFader();
        }

        protected virtual void EnableFader()
        {
            _image.enabled = true;
            if (ShouldBlockRaycasts)
                _canvasGroup.blocksRaycasts = true;
        }

        protected virtual void DisableFader()
        {
            _image.enabled = false;
            if (ShouldBlockRaycasts)
                _canvasGroup.blocksRaycasts = false;
        }

        protected virtual void StartFading(float initialAlpha, float endAlpha, float duration, MyTweenType curve, bool ignoreTimeScale)
        {
            if (!CanFadeToCurrentAlpha && _canvasGroup.alpha == endAlpha) return;

            IgnoreTimescale = ignoreTimeScale;
            EnableFader();
            _fading = true;
            _initialAlpha = initialAlpha;
            _currentTargetAlpha = endAlpha;
            _fadeStartedAt = IgnoreTimescale ? Time.unscaledTime : Time.time;
            _currentCurve = curve;
            _currentDuration = duration;

            if (Time.frameCount == 1)
                _frameCountOne = true;
        }

        public virtual void FadeIn(float duration, MyTweenType curve, bool ignoreTimeScale = true)
        {
            StartFading(InactiveAlpha, ActiveAlpha, duration, curve, ignoreTimeScale);
        }

        public virtual void FadeOut(float duration, MyTweenType curve, bool ignoreTimeScale = true)
        {
            StartFading(ActiveAlpha, InactiveAlpha, duration, curve, ignoreTimeScale);
        }

        public virtual void FadeTo(float targetAlpha, float duration, MyTweenType curve, bool ignoreTimeScale = true)
        {
            float target = targetAlpha == -1 ? ActiveAlpha : targetAlpha;
            StartFading(_canvasGroup.alpha, target, duration, curve, ignoreTimeScale);
        }

        // -------------------------
        // Event listeners
        // -------------------------

        public void OnEvent(FadeEvent e)
        {
            if (e.ID != ID) return;
            FadeTo(e.TargetAlpha, e.Duration, e.Curve, e.IgnoreTimeScale);
        }

        public void OnEvent(FadeInEvent e)
        {
            if (e.ID != ID) return;
            FadeIn(e.Duration, e.Curve, e.IgnoreTimeScale);
        }

        public void OnEvent(FadeOutEvent e)
        {
            if (e.ID != ID) return;
            FadeOut(e.Duration, e.Curve, e.IgnoreTimeScale);
        }

        public void OnEvent(FadeStopEvent e)
        {
            if (e.ID != ID) return;
            _fading = false;
            if (e.Restore)
                _canvasGroup.alpha = _initialAlpha;
        }

        // -------------------------
        // Enable / Disable
        // -------------------------

        private void OnEnable()
        {
            this.EventStartListening<FadeEvent>();
            this.EventStartListening<FadeOutEvent>();
            this.EventStartListening<FadeStopEvent>();
            this.EventStartListening<FadeInEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<FadeEvent>();
            this.EventStopListening<FadeOutEvent>();
            this.EventStopListening<FadeStopEvent>();
            this.EventStopListening<FadeInEvent>();
        }
    }
}