using System;
using UnityEngine;

public class ChaseTriggerZone : MonoBehaviour
{
    #region Events
    public event Action OnChaseBegin;
    #endregion

    #region Debug
    private void Awake()
    {
        Debug.Log($"🟢 ChaseTriggerZone.Awake() sur GameObject: {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log($"🟢 ChaseTriggerZone.Start() - Nombre d'abonnés à OnChaseBegin: {OnChaseBegin?.GetInvocationList().Length ?? 0}");
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔔 OnTriggerEnter appelé! Objet entré: '{other.name}' | Tag: '{other.tag}'");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ TAG PLAYER DÉTECTÉ!");
            
            if (OnChaseBegin != null)
            {
                Debug.Log($"🚀 Invocation de l'event OnChaseBegin ({OnChaseBegin.GetInvocationList().Length} abonné(s))");
                OnChaseBegin.Invoke();
            }
            else
            {
                Debug.LogError("❌ OnChaseBegin est NULL! Personne n'est abonné à cet event!");
            }
        }
        else
        {
            Debug.LogWarning($"❌ Tag incorrect: attendu 'Player', reçu '{other.tag}'");
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
