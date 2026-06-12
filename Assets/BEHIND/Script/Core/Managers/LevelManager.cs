using System;
using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, ILevelManager, IEventListener<GameEngineEvent>
{
    [Header("Player")]
    [SerializeField] private WheelChairController playerPrefab;
    [SerializeField] private Transform initialSpawnPoint;

    [Header("Intro / Outro")]
    public float IntroFadeDuration = 1f;
    public float OutroFadeDuration = 1f;
    public int FaderID = 0;
    public MyTweenType FadeCurve = new MyTweenType(MyTween.TweenType.EaseInOutCubic);

    [Header("Respawn")]
    public float RespawnDelay = 2f;

    private WheelChairController _spawnedPlayer;

    // -------------------------------------------------------------------------
    // Unity
    // -------------------------------------------------------------------------

    private void Awake()
    {
        GameServiceLocator.Register<ILevelManager>(this, GameServiceLocator.ServiceLifecycle.SceneScoped);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ILevelManager>();
    }

    private void Start()
    {
        StartCoroutine(InitializationCoroutine());
    }

    private void OnEnable()
    {
        this.EventStartListening<GameEngineEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<GameEngineEvent>(); // 👈 tu avais StartListening ici, c'est un bug
    }

    // -------------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------------

    private IEnumerator InitializationCoroutine()
    {
        GameEngineEvent.Trigger(GameEngineEventTypes.SpawnCharacterStarts);
        SpawnCharacter();
        yield return null;

        int id = GameServiceLocator.Get<IGameManager>().GetCheckpointID();
        if (id != -1)
        {
            foreach (var cp in FindObjectsByType<CheckPoint>(FindObjectsSortMode.None))
            {
                if (cp.CheckpointID != id) continue;

                var wheels = _spawnedPlayer.GetComponentsInChildren<WheelCollider>();
                foreach (var w in wheels) w.enabled = false;

                _spawnedPlayer.GetComponent<Rigidbody>().position = cp.transform.position;
                _spawnedPlayer.transform.position = cp.transform.position;

                yield return new WaitForFixedUpdate();

                foreach (var w in wheels) w.enabled = true;
                cp.SelectCheckPoint();
                break;
            }
        }

        GameEngineEvent.Trigger(GameEngineEventTypes.LevelStart);
        PlayerEvent.Trigger(PlayerEventTypes.PlayerSpawn, _spawnedPlayer);
        CameraEvent.Trigger(CameraEventTypes.SetTargetCharacter, _spawnedPlayer);
        CameraEvent.Trigger(CameraEventTypes.StartFollowing);

        if (id != -1)
            FadeOutEvent.Trigger(IntroFadeDuration, FadeCurve, FaderID);

        yield break;
    }

    private void SpawnCharacter()
    {
        if (playerPrefab == null || initialSpawnPoint == null)
        {
            Debug.LogError("[LevelManager] Player prefab or spawn point not assigned.");
            return;
        }
        _spawnedPlayer = Instantiate(playerPrefab, initialSpawnPoint.position, initialSpawnPoint.rotation);
    }

    // -------------------------------------------------------------------------
    // End Level
    // -------------------------------------------------------------------------

    /// <summary>
    /// Joue l'outro (fade au noir) puis charge la scène spécifiée.
    /// </summary>
    public void EndLevel(string sceneName)
    {
        GameServiceLocator.Get<IGameManager>().ResetGame();
        GameEngineEvent.Trigger(GameEngineEventTypes.LevelEnd);
        StartCoroutine(EndLevelCoroutine(sceneName));
    }

    private IEnumerator EndLevelCoroutine(string sceneName)
    {
        // On désactive le joueur pendant le fade
        //if (_spawnedPlayer != null)
        //    _spawnedPlayer.enabled = false;

        // Fade au noir
        FadeInEvent.Trigger(OutroFadeDuration, FadeCurve, FaderID);
        yield return new WaitForSeconds(OutroFadeDuration);

        LoadLevel(sceneName);
    }

    // -------------------------------------------------------------------------
    // Respawn
    // -------------------------------------------------------------------------

    private IEnumerator RespawnCoroutine()
    {
        FadeInEvent.Trigger(OutroFadeDuration, FadeCurve, FaderID);
        yield return new WaitForSeconds(OutroFadeDuration);

        yield return new WaitForSeconds(RespawnDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // -------------------------------------------------------------------------
    // Load
    // -------------------------------------------------------------------------

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    // -------------------------------------------------------------------------
    // Events
    // -------------------------------------------------------------------------

    public void OnEvent(GameEngineEvent e)
    {
        switch (e.EventType)
        {
            case GameEngineEventTypes.GameOver:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case GameEngineEventTypes.RespawnStarted:
                StartCoroutine(RespawnCoroutine());
                break;
        }
    }
}