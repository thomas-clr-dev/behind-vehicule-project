using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class MenuFocus : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    private System.IDisposable buttonPressListener;

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        buttonPressListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
        StartCoroutine(SetFocusNextFrame());
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        buttonPressListener?.Dispose();
    }

    void Update()
    {
        if (EventSystem.current == null) return;
        if (EventSystem.current.currentSelectedGameObject != null) return;

        // Détecte le mouvement de n'importe quel stick/dpad
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.leftStick.ReadValue().magnitude > 0.5f ||
                gamepad.rightStick.ReadValue().magnitude > 0.5f ||
                gamepad.dpad.ReadValue().magnitude > 0.5f)
            {
                StartCoroutine(SetFocusNextFrame());
                return;
            }
        }
    }

    private void OnAnyButtonPress(InputControl control)
    {
        if (EventSystem.current == null) return;
        if (EventSystem.current.currentSelectedGameObject != null) return;

        bool isMouse = control.device is Mouse;
        if (!isMouse) StartCoroutine(SetFocusNextFrame());
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Reconnected ||
            change == InputDeviceChange.Added)
        {
            StartCoroutine(SetFocusNextFrame());
        }
    }

   private System.Collections.IEnumerator SetFocusNextFrame()
{
    yield return null;
    yield return null; // deuxième frame pour le menu pause
    if (EventSystem.current == null || firstSelectedButton == null) yield break;

    EventSystem.current.SetSelectedGameObject(null);
    EventSystem.current.SetSelectedGameObject(firstSelectedButton);
}
}