using UnityEngine;

using UnityEngine.Audio;

// =============================================================================
// SfxEvent — déclenche un son SFX simple depuis n'importe où
// Usage : SfxEvent.Trigger(myClip);
// =============================================================================
public struct SfxEvent
{
    public delegate void Delegate(AudioClip clip, AudioMixerGroup audioGroup = null,
        float volume = 1f, float pitch = 1f, int priority = 128);

    private static event Delegate OnEvent;

    public static void Register(Delegate callback) => OnEvent += callback;
    public static void Unregister(Delegate callback) => OnEvent -= callback;

    public static void Trigger(AudioClip clip, AudioMixerGroup audioGroup = null,
        float volume = 1f, float pitch = 1f, int priority = 128)
        => OnEvent?.Invoke(clip, audioGroup, volume, pitch, priority);
}