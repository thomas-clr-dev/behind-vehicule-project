using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundManagerEventTypes
{
    SaveSettings,
    LoadSettings,
    ResetSettings,
    SettingsLoaded
}
public struct SoundManagerEvent
{
    public SoundManagerEventTypes EventType;

    public SoundManagerEvent(SoundManagerEventTypes eventType)
    {
        EventType = eventType;
    }

    static SoundManagerEvent e;

    public static void Trigger(SoundManagerEventTypes eventType)
    {
        e.EventType = eventType;
        EventBus.Publish(e);
    }

}
