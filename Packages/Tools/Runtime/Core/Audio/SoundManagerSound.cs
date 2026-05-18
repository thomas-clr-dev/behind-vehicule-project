using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundManagerSound
{
    public int ID;

    public SoundManager.SoundManagerTracks Track;

    public AudioSource Source;

    public bool Persistent; 

    public float PlaybackTime;  
    public float PlaybackDuration;
}
