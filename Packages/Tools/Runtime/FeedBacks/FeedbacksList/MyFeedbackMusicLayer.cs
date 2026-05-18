using UnityEngine;
using System;

/// <summary>
/// Feedback qui gère une couche musicale dynamique via le SoundManager.
/// - Joue le clip en boucle persistante au Init() (via le pool, Persistent=true)
/// - Fade vers le volume cible à chaque Play()
/// - La source est trackée dans _sounds donc affectée par MuteTrack, FadeTrack, etc.
/// </summary>
/// 
[Serializable]
[FeedbackPath("Audio/Music Layer")]
public class MyFeedbackMusicLayer : MyFeedback
{

    public MyFeedbackMusicLayer()
    {
        Label = "Music Layer";
        FeedbackColor = Color.green;
        Duration = 1f;
    }

    [InspectorGroup("Music Layer", true, 20)]
    [Tooltip("Le clip audio de cette couche musicale")]
    public AudioClip Clip;

    [Tooltip("ID unique pour retrouver cette couche via FindByID(). Chaque couche doit avoir un ID différent.")]
    public int LayerID = 0;

    [Tooltip("Track audio (Music recommandé pour bénéficier du volume global et du mute)")]
    public SoundManager.SoundManagerTracks Track = SoundManager.SoundManagerTracks.Music;

    [InspectorGroup("Fade", true, 51)]
    [Tooltip("Volume cible après le fade")]
    [Range(0f, 1f)]
    public float TargetVolume = 1f;

    [Tooltip("Durée du fade en secondes (0 = instantané)")]
    public float FadeDuration = 1f;

    // Référence à la source audio récupérée après PlaySound()
    // NonSerialized : pas besoin de la sauvegarder, elle est recréée au Init()
    [NonSerialized] private AudioSource _source;

    /// <summary>
    /// Au Init, on démarre la couche en boucle à volume 0 via le SoundManager.
    /// Persistent = true : la source survit à FreeAllSoundsButPersistent() (changement de scène, etc.)
    /// </summary>
    public override void Init(MyFeedbackPlayer owner)
    {
        base.Init(owner);

        if (Clip == null) return;

        var sm = GameServiceLocator.Get<IAudioManager>();
        if (sm == null) return;

        _source = sm.FindByID(LayerID);

        if (_source == null)
        {
            SoundManagerPlayOptions options = new SoundManagerPlayOptions
            {
                SoundManagerTrack = Track,
                Volume = 0f,       
                Loop = true,
                Persistent = true, 
                ID = LayerID,
                Pitch = 1f,
            };
            _source = sm.PlaySound(Clip, options);
        }
    }

    /// <summary>
    /// À chaque changement d'état de chase, fade vers le volume cible.
    /// </summary>
    protected override void CustomPlay()
    {
        if (_source == null)
        {
            var sm = GameServiceLocator.Get<IAudioManager>() as SoundManager;
            _source = sm?.FindByID(LayerID);
            if (_source == null) return;
        }

        var soundManager = GameServiceLocator.Get<IAudioManager>() as SoundManager;
        if (soundManager == null) return;

        if (FadeDuration > 0f)
            soundManager.FadeSound(_source, FadeDuration, _source.volume, TargetVolume);
        else
            _source.volume = TargetVolume;
    }
}