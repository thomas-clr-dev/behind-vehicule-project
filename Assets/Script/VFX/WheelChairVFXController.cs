

using UnityEngine;


using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
//using System.Numerics;


public class WheelVFX : MonoBehaviour

{

    public Vector3 normalized;


    public List<AudioClip> wheelList;
    public AudioSource audioWheel;

    private bool vfxAlreadyPlayed = false;

    private EventBinding<WheelStateDataEvent> dataBinding;
    

    [SerializeField] private GameObject vfxPrefab;
    private void OnEnable()
    {
        dataBinding = new EventBinding<WheelStateDataEvent>(OnDataUpdated);
        EventBus<WheelStateDataEvent>.Register(dataBinding);
    }


    private void OnDataUpdated(WheelStateDataEvent e)
    {
        //Debug.Log(e.MotorTorque);
        if(e.MotorTorque == 97  && !vfxAlreadyPlayed) 
        {
            Debug.Log("Je me lance + " +e.MotorTorque);

            GameObject go = Instantiate(vfxPrefab, transform.position, transform.rotation);

            vfxAlreadyPlayed = true;

            if(wheelList != null)
            {
                int r =Random.Range(0, wheelList.Count);
                audioWheel.clip = wheelList[r];
                audioWheel.PlayOneShot(wheelList[r], 0.5f);
            } 


            
        }

        else if (e.MotorTorque <1)
        {
            Debug.Log("Reset bool");
            vfxAlreadyPlayed = false;
        }
    }
   

   private void  OnDisable()
    {
        EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}
