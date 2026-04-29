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
        
        if (other.CompareTag("Player"))
        {
            
            if (OnChaseBegin != null)
            {
                OnChaseBegin.Invoke();
            }
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(10f, 5f, 10f));
    }
    #endregion
}
