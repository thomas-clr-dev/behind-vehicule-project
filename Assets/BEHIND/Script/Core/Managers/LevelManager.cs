using System;
using UnityEngine;

public class LevelManager : MonoBehaviour, ILevelManager
{
    [Header("Player Prefab")]
    [Tooltip("The player prefab this level manager will instantiate on Start")]
    [SerializeField] private WheelChairController playerPrefab;

    [Space(10)]
    [SerializeField] private Transform initialSpawnPoint;

    private WheelChairController spawnedPlayer;
    public void LoadLevel(string levelName)
    {
       
    }

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        GameEngineEvent.Trigger(GameEngineEventTypes.SpawnCharacterStarts);

        SpawnCharacter();

        GameEngineEvent.Trigger(GameEngineEventTypes.LevelStart);

        PlayerEvent.Trigger(PlayerEventTypes.PlayerSpawn, spawnedPlayer);   
        CameraEvent.Trigger(CameraEventTypes.SetTargetCharacter, spawnedPlayer);
        CameraEvent.Trigger(CameraEventTypes.StartFollowing);
    }

    private void SpawnCharacter()
    {
        if (playerPrefab != null && initialSpawnPoint != null)
        {
            spawnedPlayer = Instantiate(playerPrefab, initialSpawnPoint.position, initialSpawnPoint.rotation);
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
