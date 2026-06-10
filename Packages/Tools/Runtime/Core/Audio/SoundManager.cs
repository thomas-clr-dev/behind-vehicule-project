using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// =============================================================================
// SoundManager
// =============================================================================
public class SoundManager : MonoBehaviour, IAudioManager,
    IEventListener<SoundManagerTrackEvent>,
    IEventListener<SoundManagerEvent>,
    IEventListener<SoundManagerSoundControlEvent>,
    IEventListener<SoundManagerSoundFadeEvent>,
    IEventListener<SoundManagerAllSoundsControlEvent>,
    IEventListener<SoundManagerTrackFadeEvent>
{
    public enum SoundManagerTracks { Sfx, Music, UI, Master, Other }

    [Header("Settings")]
    public SoundManagerSettingsSO Settings;

    [Header("Pool")]
    public int AudioSourcePoolSize = 10;
    public bool PoolCanExpand = true;

    protected SoundManagerAudioPool _pool;
    protected SoundManagerSound _sound;
    protected List<SoundManagerSound> _sounds;
    protected AudioSource _tempAudioSource;

    protected Dictionary<AudioSource, Coroutine> _fadeInSoundCoroutines;
    protected Dictionary<AudioSource, Coroutine> _fadeOutSoundCoroutines;
    protected Dictionary<SoundManagerTracks, Coroutine> _fadeTrackCoroutines;
    protected Dictionary<SoundManagerTracks, bool> _pausedTracks = new Dictionary<SoundManagerTracks, bool>();

    // =========================================================================
    // Initialization
    // =========================================================================

    private void Awake()
    {
        GameServiceLocator.Register<IAudioManager>(this);
        Init();
    }

    /// AutoLoad au Start (après Awake) car l'AudioMixer API n'est pas prête dans Awake
    private void Start()
    {
        if (Settings != null && Settings.Settings.AutoLoad)
            Settings.LoadSoundSettings();
    }

    private void Init()
    {
        if (_pool == null) _pool = new SoundManagerAudioPool();
        _sounds = new List<SoundManagerSound>();
        _pool.FillAudioSourcePool(AudioSourcePoolSize, this.transform);
        _fadeInSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        _fadeOutSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        _fadeTrackCoroutines = new Dictionary<SoundManagerTracks, Coroutine>();
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IAudioManager>();
    }

    // =========================================================================
    // Update — nettoyage des sons terminés
    // =========================================================================

    protected virtual void Update()
    {
        _pool.CleanUp(_sounds);
    }

    // =========================================================================
    // PlaySound
    // =========================================================================

    /// Surcharge avec PlayOptions (API principale recommandée)
    public virtual AudioSource PlaySound(AudioClip audioClip, SoundManagerPlayOptions options)
    {
        return PlaySound(
            audioClip,
            options.SoundManagerTrack,
            options.Loop,
            options.Volume,
            options.ID,
            options.Fade, options.FadeInitialVolume, options.FadeDuration, options.FadeTween,
            options.Persistent,
            options.RecycleAudioSource,
            options.AudioGroup,
            options.Pitch, options.PanStereo, options.SpatialBlend,
            options.SoloSingleTrack, options.SoloAllTracks, options.AutoUnSoloOnEnd,
            options.BypassEffects, options.BypassListenerEffects, options.BypassReverbZones,
            options.Priority, options.ReverbZoneMix,
            options.DopplerLevel, options.Spread, options.RolloffMode,
            options.MinDistance, options.MaxDistance,
            options.DoNotAutoRecycleIfNotDonePlaying,
            options.PlaybackTime, options.PlaybackDuration,
            options.AttachToTransform,
            options.UseSpreadCurve, options.SpreadCurve,
            options.UseCustomRolloffCurve, options.CustomRolloffCurve,
            options.UseSpatialBlendCurve, options.SpatialBlendCurve,
            options.UseReverbZoneMixCurve, options.ReverbZoneMixCurve,
            options.InitialDelay
        );
    }

    /// Surcharge complète avec tous les paramètres
    public virtual AudioSource PlaySound(
        AudioClip audioClip,
        SoundManagerTracks soundManagerTrack,
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
        bool doNotAutoRecycleIfNotDonePlaying = false,
        float playbackTime = 0f, float playbackDuration = 0f,
        Transform attachToTransform = null,
        bool useSpreadCurve = false, AnimationCurve spreadCurve = null,
        bool useCustomRolloffCurve = false, AnimationCurve customRolloffCurve = null,
        bool useSpatialBlendCurve = false, AnimationCurve spatialBlendCurve = null,
        bool useReverbZoneMixCurve = false, AnimationCurve reverbZoneMixCurve = null,
        float initialDelay = 0f
    )
    {
        if (this == null || audioClip == null) return null;

        // --- Audio source -------------------------------------------------------
        AudioSource audioSource = recycleAudioSource;

        if (!audioSource)
        {
            audioSource = _pool.GetAvailableAudioSource(PoolCanExpand, this.transform);
            if (!audioSource)
            {
                Debug.LogWarning("[SoundManager] Pool épuisé, le son ne jouera pas. Augmente AudioSourcePoolSize ou active PoolCanExpand.");
                return null;
            }

            if (!loop)
            {
                recycleAudioSource = audioSource;
                float duration = audioClip.length / Mathf.Abs(pitch);
                StartCoroutine(_pool.AutoDisableAudioSource(
                    duration, audioSource, audioClip,
                    doNotAutoRecycleIfNotDonePlaying, playbackTime, playbackDuration));
            }
        }

        // --- Paramètres AudioSource ---------------------------------------------
        audioSource.clip = audioClip;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.panStereo = panStereo;
        audioSource.loop = loop;
        audioSource.bypassEffects = bypassEffects;
        audioSource.bypassListenerEffects = bypassListenerEffects;
        audioSource.bypassReverbZones = bypassReverbZones;
        audioSource.priority = priority;
        audioSource.reverbZoneMix = reverbZoneMix;
        audioSource.dopplerLevel = dopplerLevel;
        audioSource.spread = spread;
        audioSource.rolloffMode = rolloffMode;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.time = playbackTime;

        // Courbes personnalisées
        if (useSpreadCurve) audioSource.SetCustomCurve(AudioSourceCurveType.Spread, spreadCurve);
        if (useCustomRolloffCurve) audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve);
        if (useSpatialBlendCurve) audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialBlendCurve);
        if (useReverbZoneMixCurve) audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, reverbZoneMixCurve);

        // --- Attach to transform (son 3D qui suit un objet) ----------------------
        if (attachToTransform != null)
        {
            FollowTarget followTarget = audioSource.GetComponent<FollowTarget>();
            if (followTarget == null) followTarget = audioSource.gameObject.AddComponent<FollowTarget>();
            followTarget.Target = attachToTransform;
            followTarget.FollowMode = FollowTarget.FollowModes.Instant;
            followTarget.FollowPosition = true;
            followTarget.FollowRotation = false;
            followTarget.FollowScale = false;
            followTarget.UpdateMode = FollowTarget.UpdateModes.Update;
            followTarget.enabled = true;
        }
        else
        {
            // Désactive le FollowTarget si la source ne doit pas suivre
            FollowTarget followTarget = audioSource.GetComponent<FollowTarget>();
            if (followTarget != null) followTarget.enabled = false;
        }

        // --- Track & mixer -------------------------------------------------------
        if (Settings != null)
        {
            audioSource.outputAudioMixerGroup = Settings.MasterAudioMixerGroup;
            switch (soundManagerTrack)
            {
                case SoundManagerTracks.Master: audioSource.outputAudioMixerGroup = Settings.MasterAudioMixerGroup; break;
                case SoundManagerTracks.Music: audioSource.outputAudioMixerGroup = Settings.MusicAudioMixerGroup; break;
                case SoundManagerTracks.Sfx: audioSource.outputAudioMixerGroup = Settings.SfxAudioMixerGroup; break;
                case SoundManagerTracks.UI: audioSource.outputAudioMixerGroup = Settings.UIAudioMixerGroup; break;
            }
        }
        if (audioGroup != null) audioSource.outputAudioMixerGroup = audioGroup;

        audioSource.volume = fade ? 0f : volume;

        // --- Lecture -------------------------------------------------------------
        if (initialDelay > 0f) audioSource.PlayDelayed(initialDelay);
        else audioSource.Play();

        // --- Fade in -------------------------------------------------------------
        if (fade) FadeSound(audioSource, fadeDuration, fadeInitialVolume, volume, fadeTween);

        // --- Solo ----------------------------------------------------------------
        if (soloSingleTrack)
        {
            MuteSoundsOnTrack(soundManagerTrack, true, 0f);
            audioSource.mute = false;
            if (autoUnSoloOnEnd) MuteSoundsOnTrack(soundManagerTrack, false, audioClip.length);
        }
        else if (soloAllTracks)
        {
            MuteAllSounds();
            audioSource.mute = false;
            if (autoUnSoloOnEnd) StartCoroutine(MuteAllSoundsCoroutine(audioClip.length - playbackTime, false));
        }

        // --- Tracking ------------------------------------------------------------
        _sound = new SoundManagerSound
        {
            ID = ID,
            Track = soundManagerTrack,
            Source = audioSource,
            Persistent = persistent,
            PlaybackTime = playbackTime,
            PlaybackDuration = playbackDuration
        };

        bool alreadyIn = false;
        for (int i = 0; i < _sounds.Count; i++)
        {
            if (_sounds[i].Source == audioSource)
            {
                _sounds[i] = _sound;
                alreadyIn = true;
                break;
            }
        }
        if (!alreadyIn) _sounds.Add(_sound);

        return audioSource;
    }

    // =========================================================================
    // SoundControls
    // =========================================================================

    public virtual void PauseSound(AudioSource source) { if (source.isPlaying) source.Pause(); }
    public virtual void ResumeSound(AudioSource source) { source.Play(); }
    public virtual void StopSound(AudioSource source) { source.Stop(); }

    public virtual void FreeSound(AudioSource source)
    {
        source.Stop();
        if (!_pool.FreeSound(source))
            Destroy(source.gameObject);
    }

    // =========================================================================
    // TrackControls
    // =========================================================================

    public virtual bool IsPaused(SoundManagerTracks track)
        => _pausedTracks.TryGetValue(track, out bool v) && v;

    public virtual void MuteTrack(SoundManagerTracks track) => ControlTrack(track, ControlTrackModes.Mute, 0f);
    public virtual void UnmuteTrack(SoundManagerTracks track) => ControlTrack(track, ControlTrackModes.Unmute, 0f);
    public virtual void SetTrackVolume(SoundManagerTracks track, float volume) => ControlTrack(track, ControlTrackModes.SetVolume, volume);

    public virtual float GetTrackVolume(SoundManagerTracks track, bool mutedVolume = false)
    {
        switch (track)
        {
            case SoundManagerTracks.Master: return mutedVolume ? Settings.Settings.MutedMasterVolume : Settings.Settings.MasterVolume;
            case SoundManagerTracks.Music: return mutedVolume ? Settings.Settings.MutedMusicVolume : Settings.Settings.MusicVolume;
            case SoundManagerTracks.Sfx: return mutedVolume ? Settings.Settings.MutedSfxVolume : Settings.Settings.SfxVolume;
            case SoundManagerTracks.UI: return mutedVolume ? Settings.Settings.MutedUIVolume : Settings.Settings.UIVolume;
        }
        return 1f;
    }

    public virtual void PauseTrack(SoundManagerTracks track)
    {
        _pausedTracks[track] = true;
        foreach (var sound in _sounds)
            if (sound.Track == track) sound.Source.Pause();
    }

    public virtual void PlayTrack(SoundManagerTracks track)
    {
        _pausedTracks[track] = false;
        foreach (var sound in _sounds)
            if (sound.Track == track) sound.Source.Play();
    }

    public virtual void StopTrack(SoundManagerTracks track)
    {
        foreach (var sound in _sounds)
            if (sound.Track == track) sound.Source.Stop();
    }

    public virtual void FreeTrack(SoundManagerTracks track)
    {
        foreach (var sound in _sounds)
            if (sound.Track == track) { sound.Source.Stop(); sound.Source.gameObject.SetActive(false); }
    }

    public virtual bool HasSoundsPlaying(SoundManagerTracks track)
    {
        foreach (var sound in _sounds)
            if (sound.Track == track && sound.Source.isPlaying) return true;
        return false;
    }

    public virtual List<SoundManagerSound> GetSoundsPlaying(SoundManagerTracks track)
    {
        var list = new List<SoundManagerSound>();
        foreach (var sound in _sounds)
            if (sound.Track == track && sound.Source.isPlaying) list.Add(sound);
        return list;
    }

    public virtual bool IsMuted(SoundManagerTracks track)
    {
        switch (track)
        {
            case SoundManagerTracks.Master: return !Settings.Settings.MasterOn;
            case SoundManagerTracks.Music: return !Settings.Settings.MusicOn;
            case SoundManagerTracks.Sfx: return !Settings.Settings.SfxOn;
            case SoundManagerTracks.UI: return !Settings.Settings.UIOn;
        }
        return false;
    }

    // Raccourcis QoL
    public virtual void MuteMusic() => MuteTrack(SoundManagerTracks.Music);
    public virtual void UnmuteMusic() => UnmuteTrack(SoundManagerTracks.Music);
    public virtual void MuteSfx() => MuteTrack(SoundManagerTracks.Sfx);
    public virtual void UnmuteSfx() => UnmuteTrack(SoundManagerTracks.Sfx);
    public virtual void MuteUI() => MuteTrack(SoundManagerTracks.UI);
    public virtual void UnmuteUI() => UnmuteTrack(SoundManagerTracks.UI);
    public virtual void MuteMaster() => MuteTrack(SoundManagerTracks.Master);
    public virtual void UnmuteMaster() => UnmuteTrack(SoundManagerTracks.Master);
    public virtual void SetVolumeMusic(float v) => SetTrackVolume(SoundManagerTracks.Music, v);
    public virtual void SetVolumeSfx(float v) => SetTrackVolume(SoundManagerTracks.Sfx, v);
    public virtual void SetVolumeUI(float v) => SetTrackVolume(SoundManagerTracks.UI, v);
    public virtual void SetVolumeMaster(float v) => SetTrackVolume(SoundManagerTracks.Master, v);

    public enum ControlTrackModes { Mute, Unmute, SetVolume }
    protected virtual void ControlTrack(SoundManagerTracks track, ControlTrackModes trackMode, float volume = 0.5f)
    {
        string target = "";
        float savedVolume = 0f;

        switch (track)
        {
            case SoundManagerTracks.Master:
                target = Settings.Settings.MasterVolumeParameter;
                if (trackMode == ControlTrackModes.Mute) { Settings.TargetAudioMixer.GetFloat(target, out Settings.Settings.MutedMasterVolume); Settings.Settings.MasterOn = false; }
                else if (trackMode == ControlTrackModes.Unmute) { savedVolume = Settings.Settings.MutedMasterVolume; Settings.Settings.MasterOn = true; }
                break;
            case SoundManagerTracks.Music:
                target = Settings.Settings.MusicVolumeParameter;
                if (trackMode == ControlTrackModes.Mute) { Settings.TargetAudioMixer.GetFloat(target, out Settings.Settings.MutedMusicVolume); Settings.Settings.MusicOn = false; }
                else if (trackMode == ControlTrackModes.Unmute) { savedVolume = Settings.Settings.MutedMusicVolume; Settings.Settings.MusicOn = true; }
                break;
            case SoundManagerTracks.Sfx:
                target = Settings.Settings.SfxVolumeParameter;
                if (trackMode == ControlTrackModes.Mute) { Settings.TargetAudioMixer.GetFloat(target, out Settings.Settings.MutedSfxVolume); Settings.Settings.SfxOn = false; }
                else if (trackMode == ControlTrackModes.Unmute) { savedVolume = Settings.Settings.MutedSfxVolume; Settings.Settings.SfxOn = true; }
                break;
            case SoundManagerTracks.UI:
                target = Settings.Settings.UIVolumeParameter;
                if (trackMode == ControlTrackModes.Mute) { Settings.TargetAudioMixer.GetFloat(target, out Settings.Settings.MutedUIVolume); Settings.Settings.UIOn = false; }
                else if (trackMode == ControlTrackModes.Unmute) { savedVolume = Settings.Settings.MutedUIVolume; Settings.Settings.UIOn = true; }
                break;
        }

        switch (trackMode)
        {
            case ControlTrackModes.Mute: Settings.SetTrackVolume(track, 0f); break;
            case ControlTrackModes.Unmute: Settings.SetTrackVolume(track, Settings.MixerVolumeToNormalized(savedVolume)); break;
            case ControlTrackModes.SetVolume: Settings.SetTrackVolume(track, volume); break;
        }

        Settings.GetTrackVolumes();
        if (Settings.Settings.AutoSave) Settings.SaveSoundSettings();
    }

    // =========================================================================
    // Fades
    // =========================================================================

    public virtual void FadeTrack(SoundManagerTracks track, float duration,
        float initialVolume = 0f, float finalVolume = 1f, MyTweenType tweenType = null)
    {
        Coroutine co = StartCoroutine(FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tweenType));
        _fadeTrackCoroutines[track] = co;
    }

    public virtual void FadeSound(AudioSource source, float duration,
        float initialVolume, float finalVolume, MyTweenType tweenType = null, bool freeAfterFade = false)
    {
        Coroutine co = StartCoroutine(FadeCoroutine(source, duration, initialVolume, finalVolume, tweenType, freeAfterFade));
        if (initialVolume < finalVolume) _fadeInSoundCoroutines[source] = co;
        else _fadeOutSoundCoroutines[source] = co;
    }

    public virtual bool SoundIsFadingIn(AudioSource source)
        => _fadeInSoundCoroutines.TryGetValue(source, out var co) && co != null;

    public virtual bool SoundIsFadingOut(AudioSource source)
        => _fadeOutSoundCoroutines.TryGetValue(source, out var co) && co != null;

    public virtual void StopFadeTrack(SoundManagerTracks track)
    {
        if (_fadeTrackCoroutines.TryGetValue(track, out var co))
        {
            StopCoroutine(co);
            _fadeTrackCoroutines.Remove(track);
        }
    }

    public virtual void StopFadeSound(AudioSource source)
    {
        if (source == null) return;
        if (_fadeInSoundCoroutines.TryGetValue(source, out var co) && co != null)
        {
            StopCoroutine(co);
            _fadeInSoundCoroutines.Remove(source);
        }
        if (_fadeOutSoundCoroutines.TryGetValue(source, out co) && co != null)
        {
            StopCoroutine(co);
            _fadeOutSoundCoroutines.Remove(source);
        }
    }

    protected virtual IEnumerator FadeTrackCoroutine(SoundManagerTracks track,
        float duration, float initialVolume, float finalVolume, MyTweenType tweenType)
    {
        float startedAt = Time.unscaledTime;
        tweenType ??= new MyTweenType(MyTween.TweenType.EaseInOutQuartic);
        while (Time.unscaledTime - startedAt <= duration)
        {
            float t = Time.unscaledTime - startedAt;
            Settings.SetTrackVolume(track, MyTween.Tween(t, 0f, duration, initialVolume, finalVolume, tweenType));
            yield return null;
        }
        Settings.SetTrackVolume(track, finalVolume);
    }

    protected virtual IEnumerator FadeCoroutine(AudioSource source,
        float duration, float initialVolume, float finalVolume, MyTweenType tweenType, bool freeAfterFade = false)
    {
        float startedAt = Time.unscaledTime;
        tweenType ??= new MyTweenType(MyTween.TweenType.EaseInOutQuartic);
        while (Time.unscaledTime - startedAt <= duration)
        {
            float t = Time.unscaledTime - startedAt;
            source.volume = MyTween.Tween(t, 0f, duration, initialVolume, finalVolume, tweenType);
            yield return null;
        }
        source.volume = finalVolume;
        if (freeAfterFade) FreeSound(source);
        if (initialVolume < finalVolume) _fadeInSoundCoroutines[source] = null;
        else _fadeOutSoundCoroutines[source] = null;
    }

    // =========================================================================
    // Solo
    // =========================================================================

    public virtual void MuteSoundsOnTrack(SoundManagerTracks track, bool mute, float delay = 0f)
        => StartCoroutine(MuteSoundsOnTrackCoroutine(track, mute, delay));

    public virtual void MuteAllSounds(bool mute = true)
        => StartCoroutine(MuteAllSoundsCoroutine(0f, mute));

    protected virtual IEnumerator MuteSoundsOnTrackCoroutine(SoundManagerTracks track, bool mute, float delay)
    {
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);
        foreach (var sound in _sounds)
            if (sound.Track == track) sound.Source.mute = mute;
    }

    protected virtual IEnumerator MuteAllSoundsCoroutine(float delay, bool mute = true)
    {
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);
        foreach (var sound in _sounds) sound.Source.mute = mute;
    }

    // =========================================================================
    // AllSoundsControls
    // =========================================================================

    public virtual void PauseAllSounds()
    {
        foreach (var sound in _sounds) sound.Source.Pause();
    }

    public virtual void PlayAllSounds()
    {
        foreach (var sound in _sounds)
            if (sound.Source.isActiveAndEnabled) sound.Source.Play();
    }

    public virtual void StopAllSounds()
    {
        foreach (var sound in _sounds) sound.Source.Stop();
    }

    public virtual void FreeAllSounds()
    {
        foreach (var sound in _sounds)
            if (sound.Source != null) FreeSound(sound.Source);
    }

    public virtual void FreeAllSoundsButPersistent()
    {
        foreach (var sound in _sounds)
            if (!sound.Persistent && sound.Source != null) FreeSound(sound.Source);
    }

    public virtual void FreeAllLoopingSounds()
    {
        foreach (var sound in _sounds)
            if (sound.Source != null && sound.Source.loop) FreeSound(sound.Source);
    }

    // =========================================================================
    // Register (sources externes, ex: AudioSource sur un prefab)
    // =========================================================================

    public void RegisterExternalSource(AudioSource source, SoundManagerTracks track, int id = 0)
    {
        if (source == null) return;
        foreach (var s in _sounds) if (s.Source == source) return; // pas de doublon
        _sounds.Add(new SoundManagerSound { ID = id, Track = track, Source = source });
    }

    public void UnregisterExternalSource(AudioSource source)
    {
        if (source == null) return;
        _sounds.RemoveAll(s => s.Source == source);
    }

    // =========================================================================
    // Find
    // =========================================================================

    public virtual AudioSource FindByID(int ID)
    {
        foreach (var sound in _sounds)
            if (sound.ID == ID) return sound.Source;
        return null;
    }

    public virtual AudioSource FindByClip(AudioClip clip)
    {
        foreach (var sound in _sounds)
            if (sound.Source != null && sound.Source.clip == clip) return sound.Source;
        return null;
    }

    public virtual int CurrentlyPlayingCount(AudioClip clip)
    {
        int count = 0;
        foreach (var sound in _sounds)
            if (sound.Source != null && sound.Source.clip == clip && sound.Source.isPlaying) count++;
        return count;
    }

    // =========================================================================
    // Events
    // =========================================================================

    protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
        => FreeAllSoundsButPersistent();

    /// Handler du SfxEvent — joue un son SFX simple depuis n'importe où dans le projet
    protected virtual void OnSfxEvent(AudioClip clip, AudioMixerGroup audioGroup = null,
        float volume = 1f, float pitch = 1f, int priority = 128)
    {
        SoundManagerPlayOptions options = SoundManagerPlayOptions.Default;
        options.SoundManagerTrack = SoundManagerTracks.Sfx;
        options.AudioGroup = audioGroup;
        options.Volume = volume;
        options.Pitch = pitch;
        options.Priority = Mathf.Min(priority, 256);
        options.Loop = false;
        PlaySound(clip, options);
    }

    /// Handler du SoundManagerSoundPlayEvent — PlayOptions complet depuis n'importe où
    protected virtual AudioSource OnSoundPlayEvent(AudioClip clip, SoundManagerPlayOptions options)
        => PlaySound(clip, options);

    public void OnEvent(SoundManagerTrackEvent e)
    {
        switch (e.EventType)
        {
            case SoundManagerTrackEventTypes.MuteTrack: MuteTrack(e.Track); break;
            case SoundManagerTrackEventTypes.UnmuteTrack: UnmuteTrack(e.Track); break;
            case SoundManagerTrackEventTypes.SetVolumeTrack: SetTrackVolume(e.Track, e.Volume); break;
            case SoundManagerTrackEventTypes.PlayTrack: PlayTrack(e.Track); break;
            case SoundManagerTrackEventTypes.PauseTrack: PauseTrack(e.Track); break;
            case SoundManagerTrackEventTypes.StopTrack: StopTrack(e.Track); break;
            case SoundManagerTrackEventTypes.FreeTrack: FreeTrack(e.Track); break;
        }
    }

    public void OnEvent(SoundManagerEvent e)
    {
        switch (e.EventType)
        {
            case SoundManagerEventTypes.SaveSettings: Settings.SaveSoundSettings(); break;
            case SoundManagerEventTypes.LoadSettings: Settings.LoadSoundSettings(); break;
            case SoundManagerEventTypes.ResetSettings: Settings.ResetSoundSettings(); break;
        }
    }

    public void OnEvent(SoundManagerSoundControlEvent e)
    {
        _tempAudioSource = e.Source != null ? e.Source : FindByID(e.SoundID);
        if (_tempAudioSource == null) return;
        switch (e.ControlType)
        {
            case SoundManagerSoundControlEventTypes.Pause: PauseSound(_tempAudioSource); break;
            case SoundManagerSoundControlEventTypes.Resume: ResumeSound(_tempAudioSource); break;
            case SoundManagerSoundControlEventTypes.Stop: StopSound(_tempAudioSource); break;
            case SoundManagerSoundControlEventTypes.Free: FreeSound(_tempAudioSource); break;
        }
    }

    public void OnEvent(SoundManagerSoundFadeEvent e)
    {
        _tempAudioSource = FindByID(e.SoundID);
        switch (e.Mode)
        {
            case SoundManagerSoundFadeEvent.Modes.PlayFade:
                if (_tempAudioSource != null)
                    FadeSound(_tempAudioSource, e.FadeDuration, _tempAudioSource.volume, e.FinalVolume, e.FadeTween);
                break;
            case SoundManagerSoundFadeEvent.Modes.StopFade:
                StopFadeSound(_tempAudioSource);
                break;
        }
    }

    public void OnEvent(SoundManagerTrackFadeEvent e)
    {
        switch (e.Mode)
        {
            case SoundManagerTrackFadeEvent.Modes.PlayFade:
                FadeTrack(e.Track, e.FadeDuration, Settings.GetTrackVolume(e.Track), e.FinalVolume, e.FadeTween);
                break;
            case SoundManagerTrackFadeEvent.Modes.StopFade:
                StopFadeTrack(e.Track);
                break;
        }
    }

    public void OnEvent(SoundManagerAllSoundsControlEvent e)
    {
        switch (e.EventType)
        {
            case SoundManagerAllSoundsControlEventTypes.Pause: PauseAllSounds(); break;
            case SoundManagerAllSoundsControlEventTypes.Play: PlayAllSounds(); break;
            case SoundManagerAllSoundsControlEventTypes.Stop: StopAllSounds(); break;
            case SoundManagerAllSoundsControlEventTypes.Free: FreeAllSounds(); break;
            case SoundManagerAllSoundsControlEventTypes.FreeAllButPersistent: FreeAllSoundsButPersistent(); break;
            case SoundManagerAllSoundsControlEventTypes.FreeAllLooping: FreeAllLoopingSounds(); break;
        }
    }

    protected virtual void OnEnable()
    {
        SfxEvent.Register(OnSfxEvent);
        SoundManagerSoundPlayEvent.Register(OnSoundPlayEvent);
        this.EventStartListening<SoundManagerEvent>();
        this.EventStartListening<SoundManagerTrackEvent>();
        this.EventStartListening<SoundManagerSoundControlEvent>();
        this.EventStartListening<SoundManagerTrackFadeEvent>();
        this.EventStartListening<SoundManagerSoundFadeEvent>();
        this.EventStartListening<SoundManagerAllSoundsControlEvent>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        SfxEvent.Unregister(OnSfxEvent);
        SoundManagerSoundPlayEvent.Unregister(OnSoundPlayEvent);
        this.EventStopListening<SoundManagerEvent>();
        this.EventStopListening<SoundManagerTrackEvent>();
        this.EventStopListening<SoundManagerSoundControlEvent>();
        this.EventStopListening<SoundManagerTrackFadeEvent>();
        this.EventStopListening<SoundManagerSoundFadeEvent>();
        this.EventStopListening<SoundManagerAllSoundsControlEvent>();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}