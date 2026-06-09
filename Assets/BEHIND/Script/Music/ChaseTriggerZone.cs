using System;
using UnityEngine;

public class ChaseTriggerZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseBegin;
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