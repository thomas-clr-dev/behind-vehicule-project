using UnityEngine;
using System;

[FeedbackPath("Audio/Sound")]
[Serializable]
public class MyFeedbackSound : MyFeedback
{

    public MyFeedbackSound()
    {
        Label = "Sound";
        FeedbackColor = Color.cyan;
    }

    [InspectorGroup("Sound", true, 20)]      // 20 = Cyan 
    public AudioClip ClipToPlay;
    [Range(0, 1)] public float Volume = 1f;  

    [InspectorGroup("Options", true, 51)]    // 51 = Green
    public SoundManager.SoundManagerTracks Track;

    protected override void CustomPlay()
    {
        if (ClipToPlay == null) return;

        SoundManagerPlayOptions options = new SoundManagerPlayOptions
        {
            SoundManagerTrack = Track,
            Volume = Volume,
            Pitch = 1f,
            Loop = false
        };

        GameServiceLocator.Get<IAudioManager>().PlaySound(ClipToPlay, options);
    }
}