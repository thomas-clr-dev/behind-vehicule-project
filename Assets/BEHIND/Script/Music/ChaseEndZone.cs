using System;
using UnityEngine;
using UnityEngine.Events;

public class ChaseEndZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseEnd;

    public UnityEvent Event;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (OnChaseEnd != null)
            {
                OnChaseEnd.Invoke();
                Event?.Invoke();
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