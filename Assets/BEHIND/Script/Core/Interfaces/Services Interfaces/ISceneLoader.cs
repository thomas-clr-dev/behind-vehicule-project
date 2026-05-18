using System.Threading.Tasks;

public interface ISceneLoader
{
    Task LoadAdditiveAsync(string sceneName);
    Task UnloadAsync(string sceneName);
    Task TransitionAsync(string sceneToUnload, string sceneToLoad);
}
