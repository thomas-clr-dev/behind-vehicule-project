using UnityEngine;

public class Monster : MonoBehaviour
{
    #region SerializeField
    [Header("Chase Settings")]
    [Tooltip("Vitesse de déplacement du monstre en X")]
    [SerializeField] private float _chaseSpeed = 4f;
    #endregion

    #region Private Fields
    private bool _isChasing = false;
    [SerializeField] private ChaseTriggerZone _chaseTriggerZone;
    #endregion

    #region Initialization
    private void Start()
    {
        Debug.Log("🔵 Monster.Start() - Recherche de ChaseTriggerZone...");

        // Essayer GetComponentInChildren
        //_chaseTriggerZone = GetComponentInChildren<ChaseTriggerZone>();

        if (_chaseTriggerZone != null)
        {
            Debug.Log($"✅ ChaseTriggerZone trouvée sur: {_chaseTriggerZone.gameObject.name}");
            _chaseTriggerZone.OnChaseBegin += StartChasing;
            Debug.Log("✅ Abonnement à l'event OnChaseBegin réussi!");
        }
        else
        {
            Debug.LogError("❌ ERREUR: ChaseTriggerZone NON TROUVÉE!");
            Debug.LogError("Vérifiez que ChaseTriggerZone est un enfant de " + gameObject.name);

            // Essayer de la trouver dans toute la scène
            _chaseTriggerZone = FindObjectOfType<ChaseTriggerZone>();
            if (_chaseTriggerZone != null)
            {
                Debug.LogWarning($"⚠️ ChaseTriggerZone trouvée AILLEURS dans la scène sur: {_chaseTriggerZone.gameObject.name}");
                _chaseTriggerZone.OnChaseBegin += StartChasing;
            }
        }
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_isChasing)
        {
            transform.position += new Vector3(_chaseSpeed * Time.deltaTime, 0, 0);
        }
    }
    #endregion

    #region Chase Logic
    private void StartChasing()
    {
        Debug.Log("🏃 StartChasing() APPELÉE! _isChasing passe à TRUE");
        _isChasing = true;
    }

    public void StopChasing()
    {
        Debug.Log("🛑 Monster stopped chasing!");
        _isChasing = false;
    }
    #endregion

    #region Debug (À RETIRER APRÈS)
    private void OnDestroy()
    {
        if (_chaseTriggerZone != null)
        {
            _chaseTriggerZone.OnChaseBegin -= StartChasing;
            Debug.Log("Désabonnement de l'event OnChaseBegin");
        }
    }
    #endregion
}
