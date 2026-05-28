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
    private void Awake()
    {
        GameServiceLocator.Register<IGameManager>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IGameManager>();
    }
}
