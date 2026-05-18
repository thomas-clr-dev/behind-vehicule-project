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

    public SoundManagerSoundControlEvent(int soundID, SoundManagerSoundControlEventTypes controlType, AudioSource source)
    {
        SoundID = soundID;
        ControlType = controlType;
        Source = source;
    }

    static SoundManagerSoundControlEvent e;

    public static void Trigger(int soundID, SoundManagerSoundControlEventTypes controlType, AudioSource source)
    {
        e.SoundID = soundID;
        e.ControlType = controlType;
        e.Source = source;
        EventBus.Publish(e);
    }

}
