

using UnityEngine;


using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;


public class WheelVFX : MonoBehaviour, IEventListener<WheelStateDataEvent>
{
    [Header("Settings")]
    [Range(0f, 1f)] public float torqueThreshold = 0.9f; 
    public float cooldownTime = 0.5f; 

    [Header("VFX Anchors")]

    public Transform anchorForward; // Drag & Drop GameObject pour choisir l'endroit où le vfx de marche avant se lance
    
    public Transform anchorReverse; // Drag & Drop GameObject pour choisir l'endroit où le vfx de marche arrière se lance

    [Header("References")]
    public List<AudioClip> wheelList;
    public AudioSource audioWheel;
    [SerializeField] private GameObject vfxPrefab;

    private bool vfxAlreadyPlayed = false;
    private float lastPlayedTime = -1f;

    private void OnEnable()
    {
        this.EventStartListening<WheelStateDataEvent>();
    }

    private void OnDataUpdated(WheelStateDataEvent e)
    {
        
        float normalizedTorque = e.MotorTorque / 100f; // Normalisation (basée sur 100 par défaut)
        bool isThresholdReached = Mathf.Abs(normalizedTorque) >= torqueThreshold;

        if (isThresholdReached && !vfxAlreadyPlayed && Time.time >= lastPlayedTime + cooldownTime)
        {
            
            if (anchorForward == null || anchorReverse == null) // On vérifie que les points de référence sont bien assignés
            {
                Debug.LogError("WheelVFX: Les anchors Forward ou Reverse ne sont pas assignés dans l'inspecteur !!!");
                return;
            }

            Transform targetAnchor;


            if (normalizedTorque <= -torqueThreshold)
            {
                // MARCHE ARRIÈRE
                targetAnchor = anchorReverse;
                Debug.Log("Lancement VFX (Arrière)");
            }
            else
            {
                // MARCHE AVANT
                targetAnchor = anchorForward;
                Debug.Log("Lancement VFX (Avant)");
            }



            Instantiate(vfxPrefab, targetAnchor.position, targetAnchor.rotation); // On utilise directement la position et la rotation de l'anchor sélectionné
            
            vfxAlreadyPlayed = true;
            lastPlayedTime = Time.time; 

            /*if (wheelList != null && wheelList.Count > 0)
            {
                int r = Random.Range(0, wheelList.Count);
                audioWheel.PlayOneShot(wheelList[r], 0.5f);
            }*/
        }
        else if (Mathf.Abs(normalizedTorque) < (torqueThreshold - 0.2f)) 
        {
            vfxAlreadyPlayed = false;
        }
    }

    private void OnDisable()
    {
        this.EventStopListening<WheelStateDataEvent>();
    }

    public void OnEvent(WheelStateDataEvent eventType)
    {
        OnDataUpdated(eventType);
    }
}
