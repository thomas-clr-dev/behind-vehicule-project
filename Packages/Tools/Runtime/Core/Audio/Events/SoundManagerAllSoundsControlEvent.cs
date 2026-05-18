using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SoundManagerAllSoundsControlEventTypes { Pause, Play, Stop, Free, FreeAllButPersistent, FreeAllLooping }
public struct SoundManagerAllSoundsControlEvent 
{
    public SoundManagerAllSoundsControlEventTypes EventType;

    public SoundManagerAllSoundsControlEvent(SoundManagerAllSoundsControlEventTypes eventType)
    {
        EventType = eventType;
    }

    static SoundManagerAllSoundsControlEvent e;

    public static void Trigger(SoundManagerAllSoundsControlEventTypes eventType)
    {
        e.EventType = eventType;
        EventBus.Publish(e);
    }
}
