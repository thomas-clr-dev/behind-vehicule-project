
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WheelChairSFX : MonoBehaviour
{

    public List<AudioClip> grabList;
    public AudioSource audioGrab;
    
      private EventBinding<PushEvent> dataBinding;
      

      private void Start()
    {
        dataBinding = new EventBinding<PushEvent>(OnPush);
        EventBus<PushEvent>.Register(dataBinding);
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
        EventBus<PushEvent>.Deregister(dataBinding);
    }
}
