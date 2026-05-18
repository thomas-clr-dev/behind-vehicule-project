using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundManagerTrackEventTypes
{
    MuteTrack,
    UnmuteTrack,
    SetVolumeTrack,
    PlayTrack,
    PauseTrack,
    StopTrack,
    FreeTrack
}

public struct SoundManagerTrackEvent 
{

    public SoundManagerTrackEventTypes EventType;
    public SoundManager.SoundManagerTracks Track;
    public float Volume;
   
    public SoundManagerTrackEvent(SoundManagerTrackEventTypes eventType, SoundManager.SoundManagerTracks track, float volume)
    {
        EventType = eventType;
        Track = track;
        Volume = volume;
    }

    static SoundManagerTrackEvent e;

    public static void Trigger(SoundManagerTrackEventTypes eventType, SoundManager.SoundManagerTracks track, float volume = 1f)
    {
        e.EventType = eventType;
        e.Track = track;
        e.Volume = volume;
        EventBus.Publish(e);
    }
}
