using System;
using UnityEngine;

public class ChaseTriggerZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseBegin;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🟢 TriggerZone '{gameObject.name}' - Collision avec: {other.gameObject.name} (Tag: {other.tag})");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log($"✅ TriggerZone '{gameObject.name}' - PLAYER DÉTECTÉ! Lancement de OnChaseBegin");
            
            if (OnChaseBegin != null)
            {
                OnChaseBegin.Invoke();
            }
            else
            {
                Debug.LogError($"❌ TriggerZone '{gameObject.name}' - OnChaseBegin est NULL!");
            }
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
    #endregion
}