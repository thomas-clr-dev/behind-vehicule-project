using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    //Gamepad pad;

    //[SerializeField] private float timer;

    //[SerializeField] private float threshold;

    //[SerializeField] private bool isChased;
    //private bool vrrr; 

    //private void Start()
    //{
    //    pad = Gamepad.current;

    //    pad.SetMotorSpeeds(10000000f, 10000000000f);


    //    timer = 0;
    //    threshold = 0.5f; 

    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.name == ">Spot Infirmary Door")
    //    {
    //        pad.SetMotorSpeeds(0.5f, 0); 

    //        timer = 0;
    //        threshold = 0.2f;
    //        isChased = true; 
    //    }

    //    if (other.name == ">Spot Infirmary")
    //    {
    //        isChased = false;
    //    }

    //    if (other.name == ">Spline Infirmary>Class Chase")
    //    {
    //        //pad.SetMotorSpeeds(1f, 0);

    //        timer = 0;
    //        threshold = 1.5f;
    //        isChased = true;
    //    }

    //    if (other.name == ">Spline Class1")
    //    {
    //        isChased = false;
    //    }

    //    if (other.name == ">Spline Toilet>Roof Chase")
    //    {
    //        pad.SetMotorSpeeds(1f, 0);

    //        timer = 0;
    //        threshold = 1.5f;
    //        isChased = true;
    //    }

    //    if (other.name == ">Spline Toilet>Roof Chase End")
    //    {
    //        isChased = true;
    //    }
    //}

    //private void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer > threshold)
    //    {
    //        pad.SetMotorSpeeds(0, 0);



    //        if (isChased == true)
    //        {
    //            if (vrrr == false)
    //            {
    //                pad.SetMotorSpeeds(0.2f, 0.2f);
    //                vrrr = true;
    //                timer = 0;
    //                threshold = 0.2f;
    //            }

    //            else if (vrrr == true)
    //            {
    //                vrrr = false;
    //                timer = 0;
    //                threshold = 0.2f;
    //            }
    //        }
    //    }

        
    //}


    //public void Chased(float low,  float high, float time)
    //{
        
    //    pad.SetMotorSpeeds(low, high);

    //    timer = 0;
    //    threshold = time;


    //}
}
