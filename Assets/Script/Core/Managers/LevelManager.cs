using UnityEngine;

public class LevelManager : MonoBehaviour, ILevelManager
{
    [Header("Player Prefab")]
    [Tooltip("The player prefab this level manager will instantiate on Start")]
    [SerializeField] private WheelChairController playerPrefab;

    [Space(10)]
    [SerializeField] private string levelName;
    [SerializeField] private Transform initialSpawnPoint;

    public void LoadLevel(string levelName)
    {
       
    }

    public void Initialization()
    {
        if (playerPrefab != null && initialSpawnPoint != null)
        {
            Instantiate(playerPrefab, initialSpawnPoint.position, initialSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("Player prefab or initial spawn point is not assigned in LevelManager.");
        }
    }

    private void Awake()
    {
        GameServiceLocator.Register<ILevelManager>(this, GameServiceLocator.ServiceLifecycle.SceneScoped);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ILevelManager>();
    }
}
