using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct SoundManagerPlayOptions 
{
    [HideInInspector]
    public bool Initialized;

    [HideInInspector]
    public AudioResource AudioResourceToPlay;

    [Header("Track")]
    public SoundManager.SoundManagerTracks SoundManagerTrack;

    public AudioMixerGroup AudioGroup;

    [Header("Sound")]
    public bool Loop;
    [Range(0f, 2f)]
    public float Volume;
    [Range(-3f, 3f)]
    public float Pitch;

    public int ID;

    [Header("Fade")]
    public bool Fade;
    [Condition("Fade", true)]
    public float FadeInitialVolume;
    [Condition("Fade", true)]
    public float FadeDuration;

    [Condition("Fade", true)]
    public MyTweenType FadeTween;


    public bool Persistent;
    public AudioSource RecycleAudioSource;

    [Header("Time")]
    public float InitialDelay;
    public float PlaybackTime;
    public float PlaybackDuration;

    [Header("Solo")]
    public bool SoloSingleTrack;
    public bool SoloAllTracks;
    public bool AutoUnSoloOnEnd;

    public static SoundManagerPlayOptions Default
    {
        get
        {
            SoundManagerPlayOptions defaultOptions = new SoundManagerPlayOptions();
            defaultOptions.Initialized = true;
            defaultOptions.AudioResourceToPlay = null;
            defaultOptions.SoundManagerTrack = SoundManager.SoundManagerTracks.Sfx;
            defaultOptions.Loop = false;
            defaultOptions.Volume = 1.0f;
            defaultOptions.ID = 0;
            defaultOptions.Fade = false;
            defaultOptions.FadeInitialVolume = 0f;
            defaultOptions.FadeDuration = 1f;
            defaultOptions.FadeTween = MyTweenType.DefaultEaseInCubic;
            defaultOptions.Persistent = false;
            defaultOptions.RecycleAudioSource = null;
            defaultOptions.AudioGroup = null;
            defaultOptions.Pitch = 1f;
            defaultOptions.InitialDelay = 0f;
            defaultOptions.SoloSingleTrack = false;
            defaultOptions.SoloAllTracks = false;
            defaultOptions.AutoUnSoloOnEnd = false;
            return defaultOptions;
        }
    }
}
