using System;
using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[FeedbackPath("Audio/Sound")]
[Serializable]
public class MyFeedbackSound : MyFeedback
{
    public MyFeedbackSound()
    {
        Label = "Sound";
        FeedbackColor = new Color(0.2f, 0.8f, 1f);
        Duration = 1f;
    }

    // -------------------------------------------------------------------------
    // Sound
    // -------------------------------------------------------------------------

    [InspectorGroup("Sound", true, 20)]
    [Tooltip("Le clip audio principal à jouer.")]
    public AudioClip Sfx;

    [InspectorGroup("Random Sound", true, 21)]
    [Tooltip("Tableau de clips parmi lesquels piocher aléatoirement.")]
    public AudioClip[] RandomSfx;
    [Tooltip("Si vrai, joue les clips dans l'ordre séquentiel plutôt qu'aléatoirement.")]
    public bool SequentialOrder = false;
    [Tooltip("Si vrai, reste bloqué sur le dernier clip jusqu'à cooldown ou reset.")]
    [Condition("SequentialOrder", true)]
    public bool SequentialOrderHoldLast = false;
    [Condition("SequentialOrderHoldLast", true)]
    public float SequentialOrderHoldCooldownDuration = 2f;

    // -------------------------------------------------------------------------
    // Sound Properties
    // -------------------------------------------------------------------------

    [InspectorGroup("Sound Properties", true, 24)]
    [Range(0f, 2f)] public float MinVolume = 1f;
    [Range(0f, 2f)] public float MaxVolume = 1f;
    [Range(-3f, 3f)] public float MinPitch = 1f;
    [Range(-3f, 3f)] public float MaxPitch = 1f;

    // -------------------------------------------------------------------------
    // SoundManager Options
    // -------------------------------------------------------------------------

    [InspectorGroup("SoundManager Options", true, 28)]
    public SoundManager.SoundManagerTracks Track = SoundManager.SoundManagerTracks.Sfx;
    public int ID = 0;
    public AudioMixerGroup AudioGroup = null;
    public AudioSource RecycleAudioSource = null;
    public bool Persistent = false;
    [Tooltip("Ne joue pas si ce clip est déjà en cours de lecture.")]
    public bool DoNotPlayIfClipAlreadyPlaying = false;
    [Tooltip("Nombre max d'instances simultanées. -1 = illimité.")]
    public int MaximumConcurrentInstances = 3;
    [Tooltip("Si vrai, arrête le son quand le feedback est stoppé.")]
    public bool StopSoundOnFeedbackStop = false;

    // -------------------------------------------------------------------------
    // Fade In
    // -------------------------------------------------------------------------

    [InspectorGroup("Fade In", true, 30)]
    public bool FadeIn = false;
    [Condition("FadeIn", true)] public float FadeInInitialVolume = 0f;
    [Condition("FadeIn", true)] public float FadeInDuration = 1f;
    [Condition("FadeIn", true)] public MyTweenType FadeInTween = new MyTweenType(MyTween.TweenType.EaseInOutQuartic);

    // -------------------------------------------------------------------------
    // Fade Out
    // -------------------------------------------------------------------------

    [InspectorGroup("Fade Out", true, 31)]
    public bool FadeOutOnStop = false;
    [Condition("FadeOutOnStop", true)] public float FadeOutDuration = 1f;
    [Condition("FadeOutOnStop", true)] public MyTweenType FadeOutTween = new MyTweenType(MyTween.TweenType.EaseInOutQuartic);

    // -------------------------------------------------------------------------
    // Solo
    // -------------------------------------------------------------------------

    [InspectorGroup("Solo", true, 32)]
    public bool SoloSingleTrack = false;
    public bool SoloAllTracks = false;
    public bool AutoUnSoloOnEnd = false;

    // -------------------------------------------------------------------------
    // Spatial Settings
    // -------------------------------------------------------------------------

    [InspectorGroup("Spatial Settings", true, 33)]
    [Range(-1f, 1f)] public float PanStereo = 0f;
    [Range(0f, 1f)] public float SpatialBlend = 0f;
    [Tooltip("Si assigné, le son suivra ce Transform pendant sa lecture.")]
    public Transform AttachToTransform;

    // -------------------------------------------------------------------------
    // Effects
    // -------------------------------------------------------------------------

    [InspectorGroup("Effects", true, 36)]
    public bool BypassEffects = false;
    public bool BypassListenerEffects = false;
    public bool BypassReverbZones = false;
    [Range(0, 256)] public int Priority = 128;
    [Range(0f, 1.1f)] public float ReverbZoneMix = 1f;

    // -------------------------------------------------------------------------
    // Time Options
    // -------------------------------------------------------------------------

    [InspectorGroup("Time Options", true, 37)]
    [Tooltip("Timestamp de départ (randomisé entre Min et Max).")]
    public Vector2 PlaybackTime = Vector2.zero;
    [Tooltip("Durée de lecture (randomisé entre Min et Max). 0 = durée complète du clip.")]
    public Vector2 PlaybackDuration = Vector2.zero;

    // -------------------------------------------------------------------------
    // 3D Sound Settings
    // -------------------------------------------------------------------------

    [InspectorGroup("3D Sound Settings", true, 38)]
    [Range(0f, 5f)] public float DopplerLevel = 1f;
    [Range(0, 360)] public int Spread = 0;
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
    public float MinDistance = 1f;
    public float MaxDistance = 500f;
    public bool UseCustomRolloffCurve = false;
    [Condition("UseCustomRolloffCurve", true)] public AnimationCurve CustomRolloffCurve;
    public bool UseSpatialBlendCurve = false;
    [Condition("UseSpatialBlendCurve", true)] public AnimationCurve SpatialBlendCurve;
    public bool UseReverbZoneMixCurve = false;
    [Condition("UseReverbZoneMixCurve", true)] public AnimationCurve ReverbZoneMixCurve;
    public bool UseSpreadCurve = false;
    [Condition("UseSpreadCurve", true)] public AnimationCurve SpreadCurve;

    // -------------------------------------------------------------------------
    // État interne
    // -------------------------------------------------------------------------

    [NonSerialized] private AudioSource _playedAudioSource;
    [NonSerialized] private AudioClip _lastPlayedClip;
    [NonSerialized] private float _randomPlaybackTime;
    [NonSerialized] private float _randomPlaybackDuration;
    [NonSerialized] private int _currentSequentialIndex = 0;
    [NonSerialized] private float _lastPlayTimestamp = -999f;
    [NonSerialized] private Coroutine _fadeOutCoroutine;

    // -------------------------------------------------------------------------
    // MyFeedback overrides
    // -------------------------------------------------------------------------

    public override void Init(MyFeedbackPlayer owner)
    {
        base.Init(owner);
        _lastPlayedClip = null;
        _currentSequentialIndex = 0;
    }

    protected override void CustomPlay()
    {
        // Random clips en priorité
        if (RandomSfx != null && RandomSfx.Length > 0)
        {
            AudioClip picked = PickRandomClip();
            if (picked != null) { PlaySound(picked); return; }
        }

        if (Sfx != null) PlaySound(Sfx);
    }

    protected override void CustomStop()
    {
        if (!StopSoundOnFeedbackStop || _playedAudioSource == null) return;

        if (FadeOutOnStop)
        {
            _fadeOutCoroutine = Owner.StartCoroutine(FadeOutCo());
            return;
        }

        FreePlayedSource();
    }

    protected override void CustomReset()
    {
        _currentSequentialIndex = 0;
        _lastPlayedClip = null;
    }

    // -------------------------------------------------------------------------
    // Core
    // -------------------------------------------------------------------------

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (DoNotPlayIfClipAlreadyPlaying)
        {
            var manager = GameServiceLocator.Get<IAudioManager>();
            if (manager != null)
            {
                AudioSource existing = manager.FindByClip(Sfx);
                if (existing != null && existing.isPlaying) return;
            }
        }

        // MaximumConcurrentInstances — on passe par l'event bus, pas par le locator
        // Le SoundManager gère ça en interne via CurrentlyPlayingCount

        // Randomisation volume / pitch / temps
        float volume = UnityEngine.Random.Range(MinVolume, MaxVolume);
        float pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
        _randomPlaybackTime = UnityEngine.Random.Range(PlaybackTime.x, PlaybackTime.y);
        _randomPlaybackDuration = UnityEngine.Random.Range(PlaybackDuration.x, PlaybackDuration.y);

        // Construction des options
        SoundManagerPlayOptions options = SoundManagerPlayOptions.Default;
        options.SoundManagerTrack = Track;
        options.AudioGroup = AudioGroup;
        options.Loop = Loop;
        options.Volume = volume;
        options.Pitch = pitch;
        options.ID = ID;
        options.Fade = FadeIn;
        options.FadeInitialVolume = FadeInInitialVolume;
        options.FadeDuration = FadeInDuration;
        options.FadeTween = FadeInTween;
        options.Persistent = Persistent;
        options.RecycleAudioSource = RecycleAudioSource;
        options.InitialDelay = Delay;
        options.PlaybackTime = _randomPlaybackTime;
        options.PlaybackDuration = _randomPlaybackDuration;
        options.DoNotAutoRecycleIfNotDonePlaying = true;
        options.PanStereo = PanStereo;
        options.SpatialBlend = SpatialBlend;
        options.AttachToTransform = AttachToTransform;
        options.SoloSingleTrack = SoloSingleTrack;
        options.SoloAllTracks = SoloAllTracks;
        options.AutoUnSoloOnEnd = AutoUnSoloOnEnd;
        options.BypassEffects = BypassEffects;
        options.BypassListenerEffects = BypassListenerEffects;
        options.BypassReverbZones = BypassReverbZones;
        options.Priority = Priority;
        options.ReverbZoneMix = ReverbZoneMix;
        options.DopplerLevel = DopplerLevel;
        options.Spread = Spread;
        options.RolloffMode = RolloffMode;
        options.MinDistance = MinDistance;
        options.MaxDistance = MaxDistance;
        options.UseSpreadCurve = UseSpreadCurve;
        options.SpreadCurve = SpreadCurve;
        options.UseCustomRolloffCurve = UseCustomRolloffCurve;
        options.CustomRolloffCurve = CustomRolloffCurve;
        options.UseSpatialBlendCurve = UseSpatialBlendCurve;
        options.SpatialBlendCurve = SpatialBlendCurve;
        options.UseReverbZoneMixCurve = UseReverbZoneMixCurve;
        options.ReverbZoneMixCurve = ReverbZoneMixCurve;

        // Fire via event — zéro dépendance au locator ou au singleton
        _playedAudioSource = SoundManagerSoundPlayEvent.Trigger(clip, options);
        _lastPlayedClip = clip;
        _lastPlayTimestamp = Time.time;

        // Durée du feedback = durée effective du clip
        Duration = (_randomPlaybackDuration > 0)
            ? _randomPlaybackDuration
            : clip.length / Mathf.Abs(pitch);
    }

    // -------------------------------------------------------------------------
    // Fade Out coroutine
    // -------------------------------------------------------------------------

    private IEnumerator FadeOutCo()
    {
        if (_playedAudioSource == null) yield break;

        SoundManagerSoundFadeEvent.Trigger(
            SoundManagerSoundFadeEvent.Modes.PlayFade,
            ID,
            FadeOutDuration,
            0f,
            FadeOutTween);

        yield return new WaitForSeconds(FadeOutDuration);
        FreePlayedSource();
    }

    private void FreePlayedSource()
    {
        if (_playedAudioSource == null) return;
        SoundManagerSoundControlEvent.Trigger(SoundManagerSoundControlEventTypes.Free, _playedAudioSource);
        _playedAudioSource = null;
    }

    // -------------------------------------------------------------------------
    // Sequential / Random clip picking
    // -------------------------------------------------------------------------

    private AudioClip PickRandomClip()
    {
        if (RandomSfx == null || RandomSfx.Length == 0) return null;

        int index;

        if (!SequentialOrder)
        {
            index = UnityEngine.Random.Range(0, RandomSfx.Length);
        }
        else
        {
            index = _currentSequentialIndex;

            if (index >= RandomSfx.Length)
            {
                if (SequentialOrderHoldLast)
                {
                    index = RandomSfx.Length - 1;
                    bool cooldownElapsed = SequentialOrderHoldCooldownDuration > 0
                        && (Time.time - _lastPlayTimestamp) > SequentialOrderHoldCooldownDuration;
                    if (cooldownElapsed) _currentSequentialIndex = 0;
                }
                else
                {
                    _currentSequentialIndex = 0;
                    index = 0;
                }
            }
            else
            {
                _currentSequentialIndex++;
            }
        }

        return RandomSfx[index];
    }

    public void ResetSequentialIndex() => _currentSequentialIndex = 0;
}