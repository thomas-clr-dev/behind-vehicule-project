using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Amb_Neon : MonoBehaviour
{
    public List<AudioClip>  neonList;
    public AudioSource buzz;

    void Start()
    {
        PlaySound();
    }

    void PlaySound()
    {
        int r = Random.Range(0, neonList.Count);
        AudioClip clip = neonList[r];
        buzz.clip = clip;
        buzz.Play();
    }

}
