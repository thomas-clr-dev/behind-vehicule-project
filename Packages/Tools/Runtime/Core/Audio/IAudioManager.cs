using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioManager 
{
    AudioSource PlaySound(AudioClip clip, SoundManagerPlayOptions options);

    AudioSource FindByID(int ID);
}
