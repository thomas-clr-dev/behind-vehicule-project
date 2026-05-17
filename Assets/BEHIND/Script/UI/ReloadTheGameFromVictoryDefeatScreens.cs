using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadTheGameFromVictoryDefeatScreens : MonoBehaviour
{
    /// <summary>
    /// Recharge la scène actuelle
    /// </summary>
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
