using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public struct SoundManagerSoundFadeEvent 
{
    public enum Modes { PlayFade, StopFade }    

    public Modes Mode;

    public int SoundID; 

    public float FadeDuration;

    public float FinalVolume;

    public MyTweenType FadeTween;

    public SoundManagerSoundFadeEvent(Modes mode, int soundID, float fadeDuration, float finalVolume, MyTweenType fadeTween)
    {
        Mode = mode;
        SoundID = soundID;
        FadeDuration = fadeDuration;
        FinalVolume = finalVolume;
        FadeTween = fadeTween;
    }

    static SoundManagerSoundFadeEvent e;

    public static void Trigger(Modes mode, int soundID, float fadeDuration, float finalVolume, MyTweenType fadeTween)
    {
        e.Mode = mode;
        e.SoundID = soundID;
        e.FadeDuration = fadeDuration;
        e.FinalVolume = finalVolume;
        e.FadeTween = fadeTween;
        EventBus.Publish(e);
    }
}
