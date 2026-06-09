// =============================================================================
// SoundManagerSettings — données sérialisées (sauvegarde, volumes, flags on/off)
// =============================================================================
using System;
using UnityEngine;

[Serializable]
public class SoundManagerSettings
{
    [Header("Audio Mixer Parameters")]
    public string MasterVolumeParameter = "MasterVolume";
    public string MusicVolumeParameter = "MusicVolume";
    public string SfxVolumeParameter = "SfxVolume";
    public string UIVolumeParameter = "UIVolume";

    [Header("Volumes (0-1)")]
    public float MasterVolume = 1f;
    public float MusicVolume = 1f;
    public float SfxVolume = 1f;
    public float UIVolume = 1f;

    // Volumes sauvegardés avant mute (pour restauration)
    [HideInInspector] public float MutedMasterVolume;
    [HideInInspector] public float MutedMusicVolume;
    [HideInInspector] public float MutedSfxVolume;
    [HideInInspector] public float MutedUIVolume;

    [Header("On/Off")]
    public bool MasterOn = true;
    public bool MusicOn = true;
    public bool SfxOn = true;
    public bool UIOn = true;

    [Header("Save & Load")]
    /// Si vrai, les réglages sont sauvegardés automatiquement à chaque changement
    public bool AutoSave = false;
    /// Si vrai, les réglages sont chargés automatiquement au démarrage
    public bool AutoLoad = false;
    /// Si vrai, les réglages du mixer sont surchargés par les valeurs sauvegardées
    public bool OverrideMixerSettings = true;

    internal static float _minimalVolume = 0.0001f;
}