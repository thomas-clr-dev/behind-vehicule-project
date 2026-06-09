using System;
using UnityEngine;
using UnityEngine.Audio;


// =============================================================================
// SoundManagerSettingsSO — ScriptableObject conteneur
// =============================================================================
[CreateAssetMenu(menuName = "Audio/SoundManagerSettings")]
public class SoundManagerSettingsSO : ScriptableObject
{
    [Header("Audio Mixer")]
    public AudioMixer TargetAudioMixer;
    public AudioMixerGroup MasterAudioMixerGroup;
    public AudioMixerGroup MusicAudioMixerGroup;
    public AudioMixerGroup SfxAudioMixerGroup;
    public AudioMixerGroup UIAudioMixerGroup;

    /// Multiplicateur pour la conversion volume normalisé -> dB mixer
    public float MixerValuesMultiplier = 20f;

    [Header("Settings")]
    public SoundManagerSettings Settings;

    private const string _saveFolderName = "SoundManager/";
    private const string _saveFileName = "sound.settings";

    // -------------------------------------------------------------------------
    // Save / Load / Reset
    // -------------------------------------------------------------------------

    public virtual void SaveSoundSettings()
    {
        //ES3.Save(_saveFileName, Settings, _saveFolderName + _saveFileName);
    }

    public virtual void LoadSoundSettings()
    {
        if (!Settings.OverrideMixerSettings) return;

        //if (ES3.FileExists(_saveFolderName + _saveFileName))
        //{
        //    SoundManagerSettings loaded = ES3.Load<SoundManagerSettings>(
        //        _saveFileName, _saveFolderName + _saveFileName);

        //    if (loaded != null)
        //    {
        //        Settings = loaded;
        //        ApplyTrackVolumes();
        //    }
        //}

        SoundManagerEvent.Trigger(SoundManagerEventTypes.LoadSettings);
    }

    public virtual void ResetSoundSettings()
    {
        //if (ES3.FileExists(_saveFolderName + _saveFileName))
        //    ES3.DeleteFile(_saveFolderName + _saveFileName);
    }

    // -------------------------------------------------------------------------
    // Volume
    // -------------------------------------------------------------------------

    public virtual void SetTrackVolume(SoundManager.SoundManagerTracks track, float volume)
    {
        if (volume <= 0f) volume = SoundManagerSettings._minimalVolume;

        switch (track)
        {
            case SoundManager.SoundManagerTracks.Master:
                TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(volume));
                Settings.MasterVolume = volume;
                break;
            case SoundManager.SoundManagerTracks.Music:
                TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(volume));
                Settings.MusicVolume = volume;
                break;
            case SoundManager.SoundManagerTracks.Sfx:
                TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(volume));
                Settings.SfxVolume = volume;
                break;
            case SoundManager.SoundManagerTracks.UI:
                TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(volume));
                Settings.UIVolume = volume;
                break;
        }

        if (Settings.AutoSave) SaveSoundSettings();
    }

    public virtual float GetTrackVolume(SoundManager.SoundManagerTracks track)
    {
        float volume = 1f;
        switch (track)
        {
            case SoundManager.SoundManagerTracks.Master:
                TargetAudioMixer.GetFloat(Settings.MasterVolumeParameter, out volume); break;
            case SoundManager.SoundManagerTracks.Music:
                TargetAudioMixer.GetFloat(Settings.MusicVolumeParameter, out volume); break;
            case SoundManager.SoundManagerTracks.Sfx:
                TargetAudioMixer.GetFloat(Settings.SfxVolumeParameter, out volume); break;
            case SoundManager.SoundManagerTracks.UI:
                TargetAudioMixer.GetFloat(Settings.UIVolumeParameter, out volume); break;
        }
        return MixerVolumeToNormalized(volume);
    }

    public virtual void GetTrackVolumes()
    {
        Settings.MasterVolume = GetTrackVolume(SoundManager.SoundManagerTracks.Master);
        Settings.MusicVolume = GetTrackVolume(SoundManager.SoundManagerTracks.Music);
        Settings.SfxVolume = GetTrackVolume(SoundManager.SoundManagerTracks.Sfx);
        Settings.UIVolume = GetTrackVolume(SoundManager.SoundManagerTracks.UI);
    }

    protected virtual void ApplyTrackVolumes()
    {
        if (!Settings.OverrideMixerSettings) return;

        TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(Settings.MasterVolume));
        TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(Settings.MusicVolume));
        TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(Settings.SfxVolume));
        TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(Settings.UIVolume));

        if (!Settings.MasterOn) TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, -80f);
        if (!Settings.MusicOn) TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, -80f);
        if (!Settings.SfxOn) TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, -80f);
        if (!Settings.UIOn) TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, -80f);

        if (Settings.AutoSave) SaveSoundSettings();
    }

    // -------------------------------------------------------------------------
    // Conversions dB <-> normalisé
    // -------------------------------------------------------------------------

    public virtual float NormalizedToMixerVolume(float normalizedVolume)
        => Mathf.Log10(normalizedVolume) * MixerValuesMultiplier;

    public virtual float MixerVolumeToNormalized(float mixerVolume)
        => Mathf.Pow(10f, mixerVolume / MixerValuesMultiplier);
}