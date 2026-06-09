using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.Audio;

// =============================================================================
// SoundManagerPlayOptions
// =============================================================================
[Serializable]
public struct SoundManagerPlayOptions
{
    [HideInInspector] public bool Initialized;
    [HideInInspector] public AudioResource AudioResourceToPlay;

    [Header("Track")]
    public SoundManager.SoundManagerTracks SoundManagerTrack;
    public AudioMixerGroup AudioGroup;

    [Header("Sound")]
    public bool Loop;
    [Range(0f, 2f)] public float Volume;
    [Range(-3f, 3f)] public float Pitch;
    public int ID;

    [Header("Fade")]
    public bool Fade;
    [Condition("Fade", true)] public float FadeInitialVolume;
    [Condition("Fade", true)] public float FadeDuration;
    [Condition("Fade", true)] public MyTweenType FadeTween;

    public bool Persistent;
    public AudioSource RecycleAudioSource;

    [Header("Time")]
    public float InitialDelay;
    public float PlaybackTime;
    public float PlaybackDuration;
    public bool DoNotAutoRecycleIfNotDonePlaying;

    [Header("Spatial Settings")]
    [Range(-1f, 1f)] public float PanStereo;
    [Range(0f, 1f)] public float SpatialBlend;
    public Vector3 Location;
    public Transform AttachToTransform;

    [Header("Solo")]
    public bool SoloSingleTrack;
    public bool SoloAllTracks;
    public bool AutoUnSoloOnEnd;

    [Header("Bypass")]
    public bool BypassEffects;
    public bool BypassListenerEffects;
    public bool BypassReverbZones;
    [Range(0, 256)] public int Priority;
    [Range(0f, 1.1f)] public float ReverbZoneMix;

    [Header("3D Sound Settings")]
    [Range(0f, 5f)] public float DopplerLevel;
    [Range(0, 360)] public int Spread;
    public AudioRolloffMode RolloffMode;
    public float MinDistance;
    public float MaxDistance;

    [Header("Custom Curves")]
    public bool UseCustomRolloffCurve;
    [Condition("UseCustomRolloffCurve", true)] public AnimationCurve CustomRolloffCurve;
    public bool UseSpatialBlendCurve;
    [Condition("UseSpatialBlendCurve", true)] public AnimationCurve SpatialBlendCurve;
    public bool UseReverbZoneMixCurve;
    [Condition("UseReverbZoneMixCurve", true)] public AnimationCurve ReverbZoneMixCurve;
    public bool UseSpreadCurve;
    [Condition("UseSpreadCurve", true)] public AnimationCurve SpreadCurve;

    public static SoundManagerPlayOptions Default
    {
        get
        {
            SoundManagerPlayOptions o = new SoundManagerPlayOptions();
            o.Initialized = true;
            o.AudioResourceToPlay = null;
            o.SoundManagerTrack = SoundManager.SoundManagerTracks.Sfx;
            o.Location = Vector3.zero;
            o.Loop = false;
            o.Volume = 1f;
            o.ID = 0;
            o.Fade = false;
            o.FadeInitialVolume = 0f;
            o.FadeDuration = 1f;
            o.FadeTween = MyTweenType.DefaultEaseInCubic;
            o.Persistent = false;
            o.RecycleAudioSource = null;
            o.AudioGroup = null;
            o.Pitch = 1f;
            o.InitialDelay = 0f;
            o.PlaybackTime = 0f;
            o.PlaybackDuration = 0f;
            o.DoNotAutoRecycleIfNotDonePlaying = true;
            o.PanStereo = 0f;
            o.SpatialBlend = 0f;
            o.AttachToTransform = null;
            o.SoloSingleTrack = false;
            o.SoloAllTracks = false;
            o.AutoUnSoloOnEnd = false;
            o.BypassEffects = false;
            o.BypassListenerEffects = false;
            o.BypassReverbZones = false;
            o.Priority = 128;
            o.ReverbZoneMix = 1f;
            o.DopplerLevel = 1f;
            o.Spread = 0;
            o.RolloffMode = AudioRolloffMode.Logarithmic;
            o.MinDistance = 1f;
            o.MaxDistance = 500f;
            return o;
        }
    }
}