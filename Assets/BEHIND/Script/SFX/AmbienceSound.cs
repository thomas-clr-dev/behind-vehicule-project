using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AmbienceSound : MonoBehaviour, IEventListener<PlayerEvent>
{

    public float maxDistance = 10f; 
    public float minDistance = 2f; 

    public bool activeRandomStereo = false;

    public Collider Area;
    private GameObject Player;

    public List<AudioClip> ambList;
    public AudioSource ambSource;

    public float timeSound = 5;

    public void OnEvent(PlayerEvent e)
    {
        Player = e.TargetCharacter.gameObject;
    }


    private void OnEnable()
    {
        this.EventStartListening<PlayerEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<PlayerEvent>();
    }
    void Start()
    {
        ambSource.clip = ambList[0];
        if(!ambSource.isPlaying)
        {
            ambSource.clip = ambList[0];
            PlayAmb();
        }
        else
        {
            PlayAmb();
        }
      
    }

    void Update()
    {
        if (Player == null) return;
        Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);

        transform.position = closestPoint;



        float distance = Vector3.Distance(Player.transform.position, closestPoint);


        float t = (distance - minDistance) / (maxDistance - minDistance);        
        ambSource.spatialBlend = Mathf.Clamp(t, 0f, 1f);  // Si distance <= minDistance -> 0 (2D), Si distance >= maxDistance -> 1 (3D)
    }

    void PlayAmb()
    {
        if(!ambSource.isPlaying)
        {
            int r = Random.Range(0, ambList.Count);
            AudioClip clip = ambList[r];
            ambSource.clip = clip;
            ambSource.PlayOneShot(ambList[r], 0.5f);
            if(activeRandomStereo == true)
            {
                ambSource.panStereo = Random.Range(-0.9f, 0.9f);  // Random de son de droite à gauche
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
