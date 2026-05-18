using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISceneLoader
{

    private void Awake()
    {
        GameServiceLocator.Register<ISceneLoader>(this, GameServiceLocator.ServiceLifecycle.Persistent);
    }

    private void OnDisable()
    {
        GameServiceLocator.Unregister<ISceneLoader>();
    }
    public async Task LoadAdditiveAsync(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone)
            await Task.Yield();
    }

    public async Task UnloadAsync(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            return;

        var operation = SceneManager.UnloadSceneAsync(sceneName);
        while (!operation.isDone)
            await Task.Yield();
    }

    public async Task TransitionAsync(string sceneToUnload, string sceneToLoad)
    {
        await LoadAdditiveAsync(sceneToLoad);
        await UnloadAsync(sceneToUnload);
    }


}
