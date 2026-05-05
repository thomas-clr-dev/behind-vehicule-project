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
        throw new System.NotImplementedException();
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
