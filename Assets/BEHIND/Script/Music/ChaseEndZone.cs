using System;
using UnityEngine;

public class ChaseEndZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseEnd;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (OnChaseEnd != null)
            {
                OnChaseEnd.Invoke();
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
    #endregion
}