using UnityEngine;

public class GUIManager : MonoBehaviour, IGUIManager
{
    public void SetGameOverScreen()
    {
        
    }

    private void Awake()
    {
        GameServiceLocator.Register<IGUIManager>(this, GameServiceLocator.ServiceLifecycle.SceneScoped, debugName: "GUI Manager");
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IGUIManager>();
    }
}
