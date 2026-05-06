using UnityEngine;
using UnityEngine.InputSystem;

public class rumble : MonoBehaviour
{
    Gamepad pad; 

    void Start()
    {
        pad = Gamepad.current;
    }
    void Update()
    {
        pad.SetMotorSpeeds(10000000f, 10000000000f); 
    }
}
