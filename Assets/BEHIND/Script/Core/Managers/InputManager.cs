using UnityEngine;

public class InputManager : MonoBehaviour, IInputManager
{
    private RDPlayerActions controls;

    public bool IsInteractPressed { get; set; }

    public bool IsPausePressed => controls.Player.Pause.WasPressedThisFrame();


    private void Awake()
    {
        GameServiceLocator.Register<IInputManager>(this, GameServiceLocator.ServiceLifecycle.SceneScoped, debugName: "Input Manager");
        controls = new RDPlayerActions();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void Update()
    {
        IsInteractPressed = controls.Player.Interact.WasPressedThisFrame();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IInputManager>();
    }
}
