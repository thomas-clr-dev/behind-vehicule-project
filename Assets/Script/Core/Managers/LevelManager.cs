using UnityEngine;

public class LevelManager : MonoBehaviour, ILevelManager
{

    [SerializeField] private string levelName;
    [SerializeField] private Transform initialSpawnPoint;

    [Tooltip("The player prefab this level manager will instantiate on Start")]
    [SerializeField] private WheelChairController playerPrefab;

    private void Awake()
    {
        GameServiceLocator.Register<ILevelManager>(this, GameServiceLocator.ServiceLifecycle.SceneScoped);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ILevelManager>();
    }
}
