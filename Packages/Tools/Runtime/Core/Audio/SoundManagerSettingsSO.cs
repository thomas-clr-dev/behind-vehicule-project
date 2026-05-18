using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
[CreateAssetMenu(fileName = "SoundManagerSettingsSO", menuName = "ScriptableObjects/Tools/Audio/SoundManagerSettingsSO", order = 1)]
public class SoundManagerSettingsSO : ScriptableObject
{

    [Tooltip("the audio mixer to use when playing sounds")]
    public AudioMixer TargetAudioMixer;

    [Tooltip("the master group")]
    public AudioMixerGroup MasterAudioMixerGroup;

    [Tooltip("the group on which to play all music sounds")]
    public AudioMixerGroup MusicAudioMixerGroup;

    [Tooltip("the group on which to play all sound effects")]
    public AudioMixerGroup SfxAudioMixerGroup;

    [Tooltip("the group on which to play all UI sounds")]
    public AudioMixerGroup UIAudioMixerGroup;

    [Tooltip("the multiplier to apply when converting normalized volume values to audio mixer values")]
    public float MixerValuesMultiplier = 20;

    [Header("Settings Unfold")]

    [Tooltip("the full settings for this SoundManager")]
    public SoundManagerSettings Settings;

    protected const string _saveFolderName = "SoundManager/";
    protected const string _saveFileName = "sound.settings";

    public virtual void SaveSoundSettings()
    {
        //MMSaveLoadManager.Save(this.Settings, _saveFileName, _saveFolderName);
    }

    public virtual void LoadSoundSettings()
    {
        //if (Settings.OverrideMixerSettings)
        //{
        //    MMSoundManagerSettings settings =
        //        (MMSoundManagerSettings)MMSaveLoadManager.Load(typeof(MMSoundManagerSettings), _saveFileName,
        //            _saveFolderName);

        //    if (settings != null)
        //    {
        //        this.Settings = settings;
        //        ApplyTrackVolumes();
        //    }

        //    MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.SettingsLoaded);
        //}
    }

    public virtual void ResetSoundSettings()
    {
        //MMSaveLoadManager.DeleteSave(_saveFileName, _saveFolderName);
    }

    public virtual void SetTrackVolume(SoundManager.SoundManagerTracks track, float volume)
    {
        if (volume <= 0f)
        {
            volume = SoundManagerSettings._minimalVolume;
        }

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

        if (Settings.AutoSave)
        {
            SaveSoundSettings();
        }
    }
    
    public virtual float GetTrackVolume(SoundManager.SoundManagerTracks track)
    {
        float volume = 1f;
        switch (track)
        {
            case SoundManager.SoundManagerTracks.Master:
                TargetAudioMixer.GetFloat(Settings.MasterVolumeParameter, out volume);
                break;
            case SoundManager.SoundManagerTracks.Music:
                TargetAudioMixer.GetFloat(Settings.MusicVolumeParameter, out volume);
                break;
            case SoundManager.SoundManagerTracks.Sfx:
                TargetAudioMixer.GetFloat(Settings.SfxVolumeParameter, out volume);
                break;
            case SoundManager.SoundManagerTracks.UI:
                TargetAudioMixer.GetFloat(Settings.UIVolumeParameter, out volume);
                break;
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
        if (Settings.OverrideMixerSettings)
        {
            TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(Settings.MasterVolume));
            TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(Settings.MusicVolume));
            TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(Settings.SfxVolume));
            TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(Settings.UIVolume));

            if (!Settings.MasterOn) { TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, -80f); }
            if (!Settings.MusicOn) { TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, -80f); }
            if (!Settings.SfxOn) { TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, -80f); }
            if (!Settings.UIOn) { TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, -80f); }

            if (Settings.AutoSave)
            {
                SaveSoundSettings();
            }
        }
    }

    public virtual float NormalizedToMixerVolume(float normalizedVolume)
    {
        return Mathf.Log10(normalizedVolume) * MixerValuesMultiplier;
    }

    public virtual float MixerVolumeToNormalized(float mixerVolume)
    {
        return (float)Math.Pow(10, (mixerVolume / MixerValuesMultiplier));
    }
}
