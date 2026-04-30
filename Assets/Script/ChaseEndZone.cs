using System;
using UnityEngine;

public class ChaseEndZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseEnd;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Debug.Log($"🔴 ChaseEndZone '{gameObject.name}' initialisée");
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔔 {gameObject.name} - Trigger entered by: {other.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            Debug.Log($"✅ {gameObject.name} - PLAYER DÉTECTÉ!");

            if (OnChaseEnd != null)
            {
                Debug.Log($"🛑 {gameObject.name} - Invocation de OnChaseEnd ({OnChaseEnd.GetInvocationList().Length} abonné(s))");
                OnChaseEnd.Invoke();
            }
            else
            {
                Debug.LogError($"❌ {gameObject.name} - OnChaseEnd est NULL! Personne n'est abonné!");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ {gameObject.name} - Tag incorrect: '{other.tag}' au lieu de 'Player'");
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