using Tools;

public struct SoundManagerTrackFadeEvent 
{
    public enum Modes { PlayFade, StopFade }

    /// whether we are fading a sound, or stopping an existing fade
    public Modes Mode;

    public SoundManager.SoundManagerTracks Track;

    public float FadeDuration;
    /// the final volume to fade towards
    public float FinalVolume;
    /// the tween to use when fading
    public MyTweenType FadeTween;

    public SoundManagerTrackFadeEvent(Modes mode, SoundManager.SoundManagerTracks track, float fadeDuration, float finalVolume, MyTweenType fadeTween)
    {
        Mode = mode;
        Track = track;
        FadeDuration = fadeDuration;
        FinalVolume = finalVolume;
        FadeTween = fadeTween;
    }

    static SoundManagerTrackFadeEvent e;

    public static void Trigger(Modes mode, SoundManager.SoundManagerTracks track, float fadeDuration, float finalVolume, MyTweenType fadeTween)
    {
        e.Mode = mode;
        e.Track = track;
        e.FadeDuration = fadeDuration;
        e.FinalVolume = finalVolume;
        e.FadeTween = fadeTween;
        EventBus.Publish(e);
    }
}
