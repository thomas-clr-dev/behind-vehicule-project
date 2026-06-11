
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WheelChairSFX : MonoBehaviour, IEventListener<PushEvent>
{

    public List<AudioClip> grabList;
    public AudioSource audioGrab;
    public GameObject vfxPrefab;
    public GameObject leftHand;
    public GameObject rightHand;

    public float cooldownTime = 2f;
    private bool vfxAlreadyPlayed = false;
    private float lastPlayedTime = -2f;

      private void Start()
    {


        this.EventStartListening<PushEvent>();
    }

    private void OnPush(PushEvent data)
    {
       if (grabList != null && grabList.Count > 0)
            {
                int r = Random.Range(0, grabList.Count);
                audioGrab.PlayOneShot(grabList[r], 0.5f);

            }
        if (!vfxAlreadyPlayed && Time.time >= lastPlayedTime + cooldownTime)
        {
            Instantiate(vfxPrefab, leftHand.transform.position, leftHand.transform.rotation);
            Instantiate(vfxPrefab, rightHand.transform.position, rightHand.transform.rotation); 
        }
            
    }

    private void OnDisable()
    {
        this.EventStopListening<PushEvent>();
    }

    public void OnEvent(PushEvent eventType)
    {
       OnPush(eventType);   
    }
}
