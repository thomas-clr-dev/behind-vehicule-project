using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbienceSound_Hallway_1 : MonoBehaviour
{
     public bool activeRandomStereo = false;

    public Collider Area;
    public GameObject Player;

    public List<AudioClip> ambList;
    public AudioSource ambSource;

 
    void Start()
    {

        ambSource.clip = ambList[0];
        PlayAmb();
    }

    void Update()
    {
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);

        transform.position = closestPoint;
    }

    void PlayAmb()
    {
        if(!ambSource.isPlaying)
        {
            int r = Random.Range(0, ambList.Count);
            AudioClip clip = ambList[r];
            ambSource.clip = clip;
            ambSource.Play();

            Debug.Log("je fais du bruit");
        }
        
    }

}
