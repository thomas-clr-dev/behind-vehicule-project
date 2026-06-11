using System;
using UnityEngine;
using UnityEngine.Events;

public class ChaseTriggerZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseBegin;
    public UnityEvent OnChaseBeginUnityEvent;
    #endregion

    public bool HasTriggered = false;
    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player") )
        {
            if(HasTriggered == true)
            {
                 return;
            }


            if (OnChaseBegin != null)
            {
                OnChaseBegin.Invoke();
                OnChaseBeginUnityEvent?.Invoke();
                HasTriggered = true;
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