using UnityEngine;


public enum GameEngineEventTypes
{
    SpawnCharacterStarts,
    LevelStart,
    LevelComplete,
    LevelEnd,
    Pause,
    UnPause,
    PlayerDeath,
    SpawnComplete,
    RespawnStarted,
    RespawnComplete,
    GameOver,
    TogglePause,
    LoadNextScene,
    PauseNoMenu,
    ActivateEnemy,
    DeactivateEnemy
}

public struct GameEngineEvent : IEvent
{
    public GameEngineEventTypes EventType;
    public object Payload;



    public GameEngineEvent(GameEngineEventTypes eventType, object payload = null)
    {
        EventType = eventType;
        Payload = payload;
    }

    static GameEngineEvent e;

    public static void Trigger(GameEngineEventTypes eventType, object payload = null)
    {
        e.EventType = eventType;
        e.Payload = payload;
        EventBus.Publish(e);
    }
}

public class GameManager : MonoBehaviour, IGameManager
{
    private int checkpointID = -1;
    private static GameManager _instance;

    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        GameServiceLocator.Register<IGameManager>(this, GameServiceLocator.ServiceLifecycle.Persistent);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            GameServiceLocator.Unregister<IGameManager>();
    }

    public void SetCheckpoint(CheckPoint checkpoint)
    {
        checkpointID = checkpoint.CheckpointID;
    }

    public int GetCheckpointID() => checkpointID;
}
