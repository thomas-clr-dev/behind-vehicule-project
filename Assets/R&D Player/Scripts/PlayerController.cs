using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Paramètres de Mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;

    private RDPlayerActions controls;

    private Vector2 leftInput;
    private Vector2 rightInput;

    private void Awake()
    {
        controls = new RDPlayerActions();        

        controls.Player.LeftStick.performed += ctx => leftInput = ctx.ReadValue<Vector2>();
        controls.Player.LeftStick.canceled += ctx => leftInput = Vector2.zero;
        controls.Player.RightStick.performed += ctx => rightInput = ctx.ReadValue<Vector2>();
        controls.Player.RightStick.canceled += ctx => rightInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        float deadzone = 0.2f;

        float leftY = Mathf.Abs(leftInput.y) > deadzone ? leftInput.y : 0f;
        float rightY = Mathf.Abs(rightInput.y) > deadzone ? rightInput.y : 0f;

        float forwardAmount = (leftY + rightY) / 2f;
        transform.Translate(Vector3.forward * forwardAmount * moveSpeed * Time.deltaTime);

        float turnAmount = leftY - rightY;
        transform.Rotate(Vector3.up * turnAmount * rotationSpeed * Time.deltaTime);
    }
}