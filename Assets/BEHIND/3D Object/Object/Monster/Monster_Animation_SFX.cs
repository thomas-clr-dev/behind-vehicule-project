using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster_Animation_SFX : MonoBehaviour
{

    public AudioSource audioSource;
    public List<AudioClip> Footsteps;
    public List<AudioClip> Screams;
    public List<AudioClip> Growls;

    public void Footstep_Monster()
    {
        int r = UnityEngine.Random.Range(0, Footsteps.Count);
        audioSource.PlayOneShot(Footsteps[r]);
    }

    public void Scream_Monster()
    {
        int r = UnityEngine.Random.Range(0, Screams.Count);
        audioSource.PlayOneShot(Screams[r]);

        int rr = UnityEngine.Random.Range(0, Growls.Count);
        audioSource.PlayOneShot(Growls[rr]);
    }

    
}
