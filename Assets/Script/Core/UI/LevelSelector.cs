using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private SceneReference scene;

    [SerializeField] private bool doNotUseLevelManager;   

    public void LoadLevel()
    {
        if (doNotUseLevelManager)
        {
           SceneManager.LoadScene(scene.SceneName);
        }
        else
        {
            GameServiceLocator.Get<ILevelManager>()?.LoadLevel(scene.SceneName);
        }
    }
}
