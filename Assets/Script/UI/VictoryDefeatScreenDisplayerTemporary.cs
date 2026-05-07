using UnityEngine;

public class VictoryDefeatScreenDisplayer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;

    [Header("Game References")]
    [SerializeField] private VictoryTriggerZone _victoryTriggerZone;

    private void Start()
    {
        if (_victoryScreen != null)
            _victoryScreen.SetActive(false);

        if (_defeatScreen != null)
            _defeatScreen.SetActive(false);

        Monster.OnGameOver += ShowDefeatScreen;

        if (_victoryTriggerZone != null)
        {
            _victoryTriggerZone.OnVictory += ShowVictoryScreen;
        }
    }

    private void OnDestroy()
    {
        Monster.OnGameOver -= ShowDefeatScreen;

        if (_victoryTriggerZone != null)
        {
            _victoryTriggerZone.OnVictory -= ShowVictoryScreen;
        }
    }

    private void ShowDefeatScreen()
    {
        if (_defeatScreen != null)
        {
            _defeatScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void ShowVictoryScreen()
    {
        if (_victoryScreen != null)
        {
            _victoryScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
