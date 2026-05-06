using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AmbienceSound : MonoBehaviour
{
    public bool activeRandomStereo = false;

    public Collider Area;
    public GameObject Player;

    public List<AudioClip> ambList;
    public AudioSource ambSource;

    public float timeSound = 5;
 
    void Start()
    {
        if(!ambSource.isPlaying)
        {
            PlayAmb();
        }
    }

    void Update()
    {
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);

        transform.position = closestPoint;

        /*if(!ambSource.isPlaying)
        {
            PlayAmb();
        }*/
    }

    void PlayAmb()
    {
        if(!ambSource.isPlaying)
        {
            int r = Random.Range(0, ambList.Count);
            AudioClip clip = ambList[r];
            ambSource.clip = clip;
            ambSource.Play();
            if(activeRandomStereo == true)
            {
                ambSource.panStereo = Random.Range(-0.9f, 0.9f);
            }
            StartCoroutine(AmbPlay());
            Debug.Log("je fais du bruit");
        }
        
    }

    IEnumerator AmbPlay()
    {
        yield return new WaitForSeconds(timeSound);
        ambSource.Stop();
        PlayAmb();
    }

}
