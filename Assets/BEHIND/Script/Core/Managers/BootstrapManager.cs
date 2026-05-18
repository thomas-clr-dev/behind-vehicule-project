using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private SceneReference startScene;

    private async void Start()
    {
        await GameServiceLocator.Get<ISceneLoader>().LoadAdditiveAsync(startScene.SceneName);
    }
}
