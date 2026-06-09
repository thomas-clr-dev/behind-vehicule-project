using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundManagerSoundControlEventTypes
{
    Pause,
    Resume,
    Stop,
    Free
}

public struct SoundManagerSoundControlEvent
{
    public int SoundID;
    public SoundManagerSoundControlEventTypes ControlType;
    public AudioSource Source;

    public SoundManagerSoundControlEvent(SoundManagerSoundControlEventTypes controlType, int soundID = 0, AudioSource source = null)
    {
        ControlType = controlType;
        SoundID = soundID;
        Source = source;
    }

    static SoundManagerSoundControlEvent e;

    public static void Trigger(SoundManagerSoundControlEventTypes controlType, AudioSource source = null, int soundID = 0)
    {
        e.ControlType = controlType;
        e.Source = source;
        e.SoundID = soundID;
        EventBus.Publish(e);
    }
}
