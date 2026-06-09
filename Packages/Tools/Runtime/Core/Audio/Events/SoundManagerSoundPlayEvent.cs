using Tools;
using UnityEngine;
using UnityEngine.Audio;

public struct SoundManagerSoundPlayEvent
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RuntimeInitialization() { OnEvent = null; }

    public delegate AudioSource Delegate(AudioClip clip, SoundManagerPlayOptions options);
    private static event Delegate OnEvent;

    public static void Register(Delegate callback) => OnEvent += callback;
    public static void Unregister(Delegate callback) => OnEvent -= callback;

    /// Trigger avec PlayOptions complet
    public static AudioSource Trigger(AudioClip clip, SoundManagerPlayOptions options)
        => OnEvent?.Invoke(clip, options);

    /// Trigger simplifié avec paramètres individuels (comme le MMSoundManagerSoundPlayEvent)
    public static AudioSource Trigger(
        AudioClip audioClip,
        SoundManager.SoundManagerTracks track,
        Vector3 location,
        bool loop = false, float volume = 1f, int ID = 0,
        bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, MyTweenType fadeTween = null,
        bool persistent = false,
        AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
        float pitch = 1f, float panStereo = 0f, float spatialBlend = 0f,
        bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,
        bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false,
        int priority = 128, float reverbZoneMix = 1f,
        float dopplerLevel = 1f, int spread = 0,
        AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic,
        float minDistance = 1f, float maxDistance = 500f,
        AudioResource audioResourceToPlay = null)
    {
        SoundManagerPlayOptions options = SoundManagerPlayOptions.Default;
        options.SoundManagerTrack = track;
        options.AudioResourceToPlay = audioResourceToPlay;
        options.Location = location;
        options.Loop = loop;
        options.Volume = volume;
        options.ID = ID;
        options.Fade = fade;
        options.FadeInitialVolume = fadeInitialVolume;
        options.FadeDuration = fadeDuration;
        options.FadeTween = fadeTween;
        options.Persistent = persistent;
        options.RecycleAudioSource = recycleAudioSource;
        options.AudioGroup = audioGroup;
        options.Pitch = pitch;
        options.PanStereo = panStereo;
        options.SpatialBlend = spatialBlend;
        options.SoloSingleTrack = soloSingleTrack;
        options.SoloAllTracks = soloAllTracks;
        options.AutoUnSoloOnEnd = autoUnSoloOnEnd;
        options.BypassEffects = bypassEffects;
        options.BypassListenerEffects = bypassListenerEffects;
        options.BypassReverbZones = bypassReverbZones;
        options.Priority = priority;
        options.ReverbZoneMix = reverbZoneMix;
        options.DopplerLevel = dopplerLevel;
        options.Spread = spread;
        options.RolloffMode = rolloffMode;
        options.MinDistance = minDistance;
        options.MaxDistance = maxDistance;
        return OnEvent?.Invoke(audioClip, options);
    }
}