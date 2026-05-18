using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour,IAudioManager,
    IEventListener<SoundManagerTrackEvent>,
    IEventListener<SoundManagerEvent>,
    IEventListener<SoundManagerSoundControlEvent>,
    IEventListener<SoundManagerSoundFadeEvent>,
    IEventListener<SoundManagerAllSoundsControlEvent>,
    IEventListener<SoundManagerTrackFadeEvent>

{
    public enum SoundManagerTracks { Sfx, Music, UI, Master, Other }

    public SoundManagerSettingsSO Settings;

    public int AudioSourcePoolSize = 10;

    public bool PoolCanExpand = true;

    protected SoundManagerAudioPool _pool;
    protected GameObject _tempAudioSourceGameObject;
    protected SoundManagerSound _sound;
    protected List<SoundManagerSound> _sounds;
    protected AudioSource _tempAudioSource;
    protected Dictionary<AudioSource, Coroutine> _fadeInSoundCoroutines;
    protected Dictionary<AudioSource, Coroutine> _fadeOutSoundCoroutines;
    protected Dictionary<SoundManagerTracks, Coroutine> _fadeTrackCoroutines;
    protected Dictionary<SoundManagerTracks, bool> _pausedTracks = new Dictionary<SoundManagerTracks, bool>();

    #region Initialization
    private void Awake()
    {
        GameServiceLocator.Register<IAudioManager>(this);
        Init();
    }

    private void Init()
    {
        if (_pool == null)
        {
            _pool = new SoundManagerAudioPool();
        }
        _sounds = new List<SoundManagerSound>();
        _pool.FillAudioSourcePool(AudioSourcePoolSize, this.transform);
        _fadeInSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        _fadeOutSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
        _fadeTrackCoroutines = new Dictionary<SoundManagerTracks, Coroutine>();
    }
    #endregion

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IAudioManager>();
    }

    #region PlaySound
    public virtual AudioSource PlaySound(AudioClip audioClip, SoundManagerPlayOptions options)
    {
        return PlaySound(audioClip, options.SoundManagerTrack, options.Loop, options.Volume, options.ID,
                options.Fade, options.FadeInitialVolume, options.FadeDuration, options.FadeTween,
                options.Persistent,
                options.RecycleAudioSource,options.AudioGroup, 
                options.Pitch, options.InitialDelay, 
                options.SoloSingleTrack, options.SoloAllTracks, options.AutoUnSoloOnEnd);
    }


    private AudioSource PlaySound(AudioClip audioClip, SoundManagerTracks soundManagerTrack,
        bool loop = false, float volume = 1.0f, int ID = 0, bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, MyTweenType fadeTween = null,
        bool persistent = false, AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
        float pitch = 1f, float initialDelay = 0f, bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,
        AudioResource audioResourceToPlay = null)
    {
        if (this == null) { return null; }
        if (!audioClip && !audioResourceToPlay) { return null; }

        AudioSource audioSource = _pool.GetAvailableAudioSource(PoolCanExpand, this.transform);
        if (!audioSource) return null;

        audioSource.clip = audioClip;
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.volume = fade ? 0f : volume;

        if (initialDelay > 0f) audioSource.PlayDelayed(initialDelay);
        else audioSource.Play();

        if (!loop)
        {
            float duration = audioClip.length / Mathf.Abs(pitch);
            StartCoroutine(_pool.AutoDisableAudioSource(duration, audioSource, audioClip));
        }

        SoundManagerSound newSound = new SoundManagerSound { ID = ID, Track = soundManagerTrack, Source = audioSource };
        _sounds.Add(newSound);

        if (fade) FadeSound(audioSource, fadeDuration, 0f, volume);

        return audioSource;

    }
    #endregion

    #region SoundConrols

    public virtual void PauseSound(AudioSource source)
    {
        if (source.isPlaying)
        {
            source.Pause();
        }
    }

    public virtual void ResumeSound(AudioSource source)
    {
        source.Play();
    }

    public virtual void StopSound(AudioSource source)
    {
        source.Stop();
    }

    public virtual void FreeSound(AudioSource source)
    {
        source.Stop();
        if (!_pool.FreeSound(source))
        {
            Destroy(source.gameObject);
        }
    }
    #endregion

    #region TrackControls

    public virtual bool IsPaused(SoundManagerTracks track)
    {
        if (_pausedTracks.TryGetValue(track, out bool muted))
        {
            return muted;
        }

        return false;
    }


    public virtual void MuteTrack(SoundManagerTracks track)
    {
        ControlTrack(track, ControlTrackModes.Mute, 0f);
    }


    public virtual void UnmuteTrack(SoundManagerTracks track)
    {
        ControlTrack(track, ControlTrackModes.Unmute, 0f);
    }

    public virtual void SetTrackVolume(SoundManagerTracks track, float volume)
    {
        ControlTrack(track, ControlTrackModes.SetVolume, volume);
    }

    public virtual float GetTrackVolume(SoundManagerTracks track, bool mutedVolume)
    {
        switch (track)
        {
            case SoundManagerTracks.Master:
                if (mutedVolume)
                {
                    return Settings.Settings.MutedMasterVolume;
                }
                else
                {
                    return Settings.Settings.MasterVolume;
                }
            case SoundManagerTracks.Music:
                if (mutedVolume)
                {
                    return Settings.Settings.MutedMusicVolume;
                }
                else
                {
                    return Settings.Settings.MusicVolume;
                }
            case SoundManagerTracks.Sfx:
                if (mutedVolume)
                {
                    return Settings.Settings.MutedSfxVolume;
                }
                else
                {
                    return Settings.Settings.SfxVolume;
                }
            case SoundManagerTracks.UI:
                if (mutedVolume)
                {
                    return Settings.Settings.MutedUIVolume;
                }
                else
                {
                    return Settings.Settings.UIVolume;
                }
        }

        return 1f;
    }


    public virtual void PauseTrack(SoundManagerTracks track)
    {
        _pausedTracks[track] = true;
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Track == track)
            {
                sound.Source.Pause();
            }
        }
    }


    public virtual void PlayTrack(SoundManagerTracks track)
    {
        _pausedTracks[track] = false;
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Track == track)
            {
                sound.Source.Play();
            }
        }
    }

    public virtual void StopTrack(SoundManagerTracks track)
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Track == track)
            {
                sound.Source.Stop();
            }
        }
    }

    public virtual bool HasSoundsPlaying(SoundManagerTracks track)
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if ((sound.Track == track) && (sound.Source.isPlaying))
            {
                return true;
            }
        }
        return false;
    }


    public virtual List<SoundManagerSound> GetSoundsPlaying(SoundManagerTracks track)
    {
        List<SoundManagerSound> soundsPlaying = new List<SoundManagerSound>();
        foreach (SoundManagerSound sound in _sounds)
        {
            if ((sound.Track == track) && (sound.Source.isPlaying))
            {
                soundsPlaying.Add(sound);
            }
        }
        return soundsPlaying;
    }

    public virtual void FreeTrack(SoundManagerTracks track)
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Track == track)
            {
                sound.Source.Stop();
                sound.Source.gameObject.SetActive(false);
            }
        }
    }

    public virtual void MuteMusic() { MuteTrack(SoundManagerTracks.Music); }

    public virtual void UnmuteMusic() { UnmuteTrack(SoundManagerTracks.Music); }

    public virtual void MuteSfx() { MuteTrack(SoundManagerTracks.Sfx); }

    public virtual void UnmuteSfx() { UnmuteTrack(SoundManagerTracks.Sfx); }

    public virtual void MuteUI() { MuteTrack(SoundManagerTracks.UI); }

    public virtual void UnmuteUI() { UnmuteTrack(SoundManagerTracks.UI); }

    public virtual void MuteMaster() { MuteTrack(SoundManagerTracks.Master); }
  
    public virtual void UnmuteMaster() { UnmuteTrack(SoundManagerTracks.Master); }
 
    public virtual void SetVolumeMusic(float newVolume) { SetTrackVolume(SoundManagerTracks.Music, newVolume); }

    public virtual void SetVolumeSfx(float newVolume) { SetTrackVolume(SoundManagerTracks.Sfx, newVolume); }

    public virtual void SetVolumeUI(float newVolume) { SetTrackVolume(SoundManagerTracks.UI, newVolume); }

    public virtual void SetVolumeMaster(float newVolume) { SetTrackVolume(SoundManagerTracks.Master, newVolume); }


    public virtual bool IsMuted(SoundManagerTracks track)
    {
        switch (track)
        {
            case SoundManagerTracks.Master:
                return !Settings.Settings.MasterOn;
            case    SoundManagerTracks.Music:
                return !Settings.Settings.MusicOn;
            case SoundManagerTracks.Sfx:
                return !Settings.Settings.SfxOn;
            case SoundManagerTracks.UI:
                return !Settings.Settings.UIOn;
        }
        return false;
    }

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
            case ControlTrackModes.Mute:
                Settings.SetTrackVolume(track, 0f);
                break;
            case ControlTrackModes.Unmute:
                Settings.SetTrackVolume(track, Settings.MixerVolumeToNormalized(savedVolume));
                break;
            case ControlTrackModes.SetVolume:
                Settings.SetTrackVolume(track, volume);
                break;
        }

        Settings.GetTrackVolumes();

        if (Settings.Settings.AutoSave)
        {
            Settings.SaveSoundSettings();
        }
    }
    #endregion

    #region Fades

    public virtual void FadeTrack(SoundManagerTracks track, float duration, float initialVolume = 0f, float finalVolume = 1f, MyTweenType tweenType = null)
    {
        Coroutine coroutine = StartCoroutine(FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tweenType));
        _fadeTrackCoroutines[track] = coroutine;
    }


    public virtual void FadeSound(AudioSource source, float duration, float initialVolume, float finalVolume, MyTweenType tweenType, bool freeAfterFade = false)
    {
        Coroutine coroutine = StartCoroutine(FadeCoroutine(source, duration, initialVolume, finalVolume, tweenType, freeAfterFade));
        if (initialVolume < finalVolume)
        {
            _fadeInSoundCoroutines[source] = coroutine;
        }
        else
        {
            _fadeOutSoundCoroutines[source] = coroutine;
        }
    }

    public virtual bool SoundIsFadingIn(AudioSource source)
    {
        if (_fadeInSoundCoroutines.TryGetValue(source, out Coroutine co))
        {
            return (_fadeInSoundCoroutines[source] != null);
        }

        return false;
    }


    public virtual bool SoundIsFadingOut(AudioSource source)
    {
        if (_fadeOutSoundCoroutines.TryGetValue(source, out Coroutine co))
        {
            return (_fadeOutSoundCoroutines[source] != null);
        }

        return false;
    }

    public virtual void StopFadeTrack(SoundManagerTracks track)
    {
        Coroutine outCoroutine;
        if (_fadeTrackCoroutines.TryGetValue(track, out outCoroutine))
        {
            StopCoroutine(outCoroutine);
            _fadeTrackCoroutines.Remove(track);
        }
    }

    public virtual void StopFadeSound(AudioSource source)
    {
        Coroutine outCoroutine;
        if ((source != null) && (_fadeInSoundCoroutines.TryGetValue(source, out outCoroutine)))
        {
            if (outCoroutine != null)
            {
                StopCoroutine(outCoroutine);
                _fadeInSoundCoroutines.Remove(source);
            }
        }
        if ((source != null) && (_fadeOutSoundCoroutines.TryGetValue(source, out outCoroutine)))
        {
            if (outCoroutine != null)
            {
                StopCoroutine(outCoroutine);
                _fadeOutSoundCoroutines.Remove(source);
            }
        }
    }

    protected virtual IEnumerator FadeTrackCoroutine(SoundManagerTracks track, float duration, float initialVolume, float finalVolume, MyTweenType tweenType)
    {
        float startedAt = Time.unscaledTime;
        if (tweenType == null)
        {
            tweenType = new MyTweenType(MyTween.TweenType.EaseInOutQuartic);
        }
        while (Time.unscaledTime - startedAt <= duration)
        {
            float elapsedTime = Time.unscaledTime - startedAt;
            float newVolume = MyTween.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
            Settings.SetTrackVolume(track, newVolume);
            yield return null;
        }
        Settings.SetTrackVolume(track, finalVolume);
    }


    protected virtual IEnumerator FadeCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume, MyTweenType tweenType, bool freeAfterFade = false)
    {
        float startedAt = Time.unscaledTime;
        if (tweenType == null)
        {
            tweenType = new MyTweenType(MyTween.TweenType.EaseInOutQuartic);
        }
        while (Time.unscaledTime - startedAt <= duration)
        {
            float elapsedTime = Time.unscaledTime - startedAt;
            float newVolume = MyTween.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
            source.volume = newVolume;
            yield return null;
        }
        source.volume = finalVolume;

        if (freeAfterFade)
        {
            FreeSound(source);
        }

        if (initialVolume < finalVolume)
        {
            _fadeInSoundCoroutines[source] = null;
        }
        else
        {
            _fadeOutSoundCoroutines[source] = null;
        }
    }
    public virtual void FadeSound(AudioSource source, float duration, float initialVolume, float finalVolume)
    {
        StartCoroutine(FadeSoundCoroutine(source, duration, initialVolume, finalVolume));
    }

    protected virtual IEnumerator FadeSoundCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume)
    {
        float startedAt = Time.unscaledTime;
        while (Time.unscaledTime - startedAt <= duration)
        {
            float elapsedTime = Time.unscaledTime - startedAt;
            source.volume = Mathf.Lerp(initialVolume, finalVolume, elapsedTime / duration);
            yield return null;
        }
        source.volume = finalVolume;
    }
    #endregion

    #region Solo

    public virtual void MuteSoundsOnTrack(SoundManagerTracks track, bool mute, float delay = 0f)
    {
        StartCoroutine(MuteSoundsOnTrackCoroutine(track, mute, delay));
    }


    public virtual void MuteAllSounds(bool mute = true)
    {
        StartCoroutine(MuteAllSoundsCoroutine(0f, mute));
    }

    protected virtual IEnumerator MuteSoundsOnTrackCoroutine(SoundManagerTracks track, bool mute, float delay)
    {
        if (delay > 0)
        {
            yield return MyCoroutine.WaitForUnscaled(delay);
        }

        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Track == track)
            {
                sound.Source.mute = mute;
            }
        }
    }

    protected virtual IEnumerator MuteAllSoundsCoroutine(float delay, bool mute = true)
    {
        if (delay > 0)
        {
            yield return MyCoroutine.WaitForUnscaled(delay);
        }
        foreach (SoundManagerSound sound in _sounds)
        {
            sound.Source.mute = mute;
        }
    }
    #endregion

    #region Register 
    public void RegisterExternalSource(AudioSource source, SoundManagerTracks track, int id = 0)
    {
        if (source == null) return;

        // Évite les doublons
        foreach (var s in _sounds)
            if (s.Source == source) return;

        _sounds.Add(new SoundManagerSound { ID = id, Track = track, Source = source });
    }

    public void UnregisterExternalSource(AudioSource source)
    {
        if (source == null) return;
        _sounds.RemoveAll(s => s.Source == source);
    }

    #endregion

    #region Find


    public virtual AudioSource FindByID(int ID)
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.ID == ID)
            {
                return sound.Source;
            }
        }

        return null;
    }


    public virtual AudioSource FindByClip(AudioClip clip)
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if ((sound.Source != null) && (sound.Source.clip == clip))
            {
                return sound.Source;
            }
        }

        return null;
    }

    public virtual int CurrentlyPlayingCount(AudioClip clip)
    {
        int count = 0;
        foreach (SoundManagerSound sound in _sounds)
        {
            if ((sound.Source != null) && (sound.Source.clip == clip) && (sound.Source.isPlaying))
            {
                count++;
            }
        }
        return count;
    }
    #endregion

    #region AllSoundsControl    

    public virtual void PauseAllSounds()
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            sound.Source.Pause();
        }
    }

    public virtual void PlayAllSounds()
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Source.isActiveAndEnabled)
            {
                sound.Source.Play();
            }
        }
    }

    public virtual void StopAllSounds()
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            sound.Source.Stop();
        }
    }

    public virtual void FreeAllSounds()
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if (sound.Source != null)
            {
                FreeSound(sound.Source);
            }
        }
    }

    public virtual void FreeAllSoundsButPersistent()
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if ((!sound.Persistent) && (sound.Source != null))
            {
                FreeSound(sound.Source);
            }
        }
    }


    public virtual void FreeAllLoopingSounds()
    {
        foreach (SoundManagerSound sound in _sounds)
        {
            if ((sound.Source.loop) && (sound.Source != null))
            {
                FreeSound(sound.Source);
            }
        }
    }

    #endregion

    #region Events

    protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        FreeAllSoundsButPersistent();
    }

    public void OnEvent(SoundManagerTrackEvent soundManagerTrackEvent)
    {
        switch (soundManagerTrackEvent.EventType)
        {
            case SoundManagerTrackEventTypes.MuteTrack:
                MuteTrack(soundManagerTrackEvent.Track);
                break;
            case SoundManagerTrackEventTypes.UnmuteTrack:
                UnmuteTrack(soundManagerTrackEvent.Track);
                break;
            case SoundManagerTrackEventTypes.SetVolumeTrack:
                SetTrackVolume(soundManagerTrackEvent.Track, soundManagerTrackEvent.Volume);
                break;
            case SoundManagerTrackEventTypes.PlayTrack:
                PlayTrack(soundManagerTrackEvent.Track);
                break;
            case SoundManagerTrackEventTypes.PauseTrack:
                PauseTrack(soundManagerTrackEvent.Track);
                break;
            case SoundManagerTrackEventTypes.StopTrack:
                StopTrack(soundManagerTrackEvent.Track);
                break;
            case SoundManagerTrackEventTypes.FreeTrack:
                FreeTrack(soundManagerTrackEvent.Track);
                break;
        }
    }

    public void OnEvent(SoundManagerEvent soundManagerEvent)
    {
        switch (soundManagerEvent.EventType)
        {
            case SoundManagerEventTypes.SaveSettings:
                Settings.SaveSoundSettings();
                break;
            case SoundManagerEventTypes.LoadSettings:
                Settings.LoadSoundSettings();
                break;
            case SoundManagerEventTypes.ResetSettings:
                Settings.ResetSoundSettings();
                break;
        }
    }

    public virtual void OnEvent(SoundManagerSoundControlEvent soundControlEvent)
    {
        if (soundControlEvent.Source == null)
        {
            _tempAudioSource = FindByID(soundControlEvent.SoundID);
        }
        else
        {
            _tempAudioSource = soundControlEvent.Source;
        }

        if (_tempAudioSource != null)
        {
            switch (soundControlEvent.ControlType)
            {
                case SoundManagerSoundControlEventTypes.Pause:
                    PauseSound(_tempAudioSource);
                    break;
                case SoundManagerSoundControlEventTypes.Resume:
                    ResumeSound(_tempAudioSource);
                    break;
                case SoundManagerSoundControlEventTypes.Stop:
                    StopSound(_tempAudioSource);
                    break;
                case SoundManagerSoundControlEventTypes.Free:
                    FreeSound(_tempAudioSource);
                    break;
            }
        }
    }

    public virtual void OnEvent(SoundManagerTrackFadeEvent trackFadeEvent)
    {
        switch (trackFadeEvent.Mode)
        {
            case SoundManagerTrackFadeEvent.Modes.PlayFade:
                FadeTrack(trackFadeEvent.Track, trackFadeEvent.FadeDuration, Settings.GetTrackVolume(trackFadeEvent.Track), trackFadeEvent.FinalVolume, trackFadeEvent.FadeTween);
                break;
            case SoundManagerTrackFadeEvent.Modes.StopFade:
                StopFadeTrack(trackFadeEvent.Track);
                break;
        }
    }

    public virtual void OnEvent(SoundManagerSoundFadeEvent soundFadeEvent)
    {
        _tempAudioSource = FindByID(soundFadeEvent.SoundID);
        switch (soundFadeEvent.Mode)
        {
            case SoundManagerSoundFadeEvent.Modes.PlayFade:
                if (_tempAudioSource != null)
                {
                    FadeSound(_tempAudioSource, soundFadeEvent.FadeDuration, _tempAudioSource.volume, soundFadeEvent.FinalVolume,
                        soundFadeEvent.FadeTween);
                }
                break;
            case SoundManagerSoundFadeEvent.Modes.StopFade:
                StopFadeSound(_tempAudioSource);
                break;
        }
    }

    public virtual void OnEvent(SoundManagerAllSoundsControlEvent allSoundsControlEvent)
    {
        switch (allSoundsControlEvent.EventType)
        {
            case SoundManagerAllSoundsControlEventTypes.Pause:
                PauseAllSounds();
                break;
            case SoundManagerAllSoundsControlEventTypes.Play:
                PlayAllSounds();
                break;
            case SoundManagerAllSoundsControlEventTypes.Stop:
                StopAllSounds();
                break;
            case SoundManagerAllSoundsControlEventTypes.Free:
                FreeAllSounds();
                break;
            case SoundManagerAllSoundsControlEventTypes.FreeAllButPersistent:
                FreeAllSoundsButPersistent();
                break;
            case SoundManagerAllSoundsControlEventTypes.FreeAllLooping:
                FreeAllLoopingSounds();
                break;
        }
    }

    protected virtual void OnEnable()
    {
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
            this.EventStopListening<SoundManagerEvent>();
            this.EventStopListening<SoundManagerTrackEvent>();
            this.EventStopListening<SoundManagerSoundControlEvent>();
            this.EventStopListening<SoundManagerTrackFadeEvent>();
            this.EventStopListening<SoundManagerSoundFadeEvent>();
            this.EventStopListening<SoundManagerAllSoundsControlEvent>();

            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion


}
