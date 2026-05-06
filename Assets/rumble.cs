using UnityEngine;
using UnityEngine.InputSystem;

public class rumble : MonoBehaviour
{
    Gamepad pad;

    [SerializeField] private float timer;

    [SerializeField] private float threshold;

    void Start()
    {
        pad = Gamepad.current;
    }
    void Update()
    {
        pad.SetMotorSpeeds(10000000f, 10000000000f);

        timer += Time.deltaTime;

        if(timer > threshold)
        {
            timer = 0;
        }
    }
}
