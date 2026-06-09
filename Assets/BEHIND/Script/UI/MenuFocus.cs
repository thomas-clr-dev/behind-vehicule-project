using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuFocus : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        SetFocus();
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void Update()
    {
        // Sécurité : si le focus est perdu, le restaurer
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            SetFocus();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Reconnected ||
            change == InputDeviceChange.Added)
        {
            SetFocus();
        }
    }

    private void SetFocus()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}