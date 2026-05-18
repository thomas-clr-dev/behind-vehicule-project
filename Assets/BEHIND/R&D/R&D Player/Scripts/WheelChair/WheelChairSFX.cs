
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WheelChairSFX : MonoBehaviour, IEventListener<PushEvent>
{

    public List<AudioClip> grabList;
    public AudioSource audioGrab;
       

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
