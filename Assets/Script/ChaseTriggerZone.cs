using System;
using UnityEngine;

public class ChaseTriggerZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseBegin;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Debug.Log($"🟢 ChaseTriggerZone '{gameObject.name}' initialisée");
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔔 {gameObject.name} - Trigger entered by: {other.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            Debug.Log($"✅ {gameObject.name} - PLAYER DÉTECTÉ!");

            if (OnChaseBegin != null)
            {
                Debug.Log($"🚀 {gameObject.name} - Invocation de OnChaseBegin ({OnChaseBegin.GetInvocationList().Length} abonné(s))");
                OnChaseBegin.Invoke();
            }
            else
            {
                Debug.LogError($"❌ {gameObject.name} - OnChaseBegin est NULL! Personne n'est abonné!");
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
    #endregion
}