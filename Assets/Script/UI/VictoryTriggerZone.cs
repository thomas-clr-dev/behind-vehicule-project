using System;
using UnityEngine;

public class VictoryTriggerZone : MonoBehaviour
{
    #region Events
    public event Action OnVictory;
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (OnVictory != null)
            {
                OnVictory.Invoke();
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
            Gizmos.color = Color.purple;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
    #endregion
}
