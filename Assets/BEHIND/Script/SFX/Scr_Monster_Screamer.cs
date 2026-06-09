using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Monster_Screamer : MonoBehaviour, IEventListener<PlayerEvent>
{

    public List<AudioClip>  screamerList;
    public AudioSource screamer;

    public GameObject  Player;

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

    public void OnTriggerEnter(Collider other)
    {
        if (Player != null)
        {
            Debug.Log("Je Scare");
            int r = Random.Range(0, screamerList.Count);
            screamer.PlayOneShot(screamerList[r], 0.5f);     
            gameObject.GetComponent<BoxCollider>().enabled = false;    
        }

    }
}
