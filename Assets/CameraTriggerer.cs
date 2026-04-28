using Unity.Cinemachine;
using UnityEngine;

public class CameraTriggerer : MonoBehaviour
{
    public CinemachineCamera actualCamera;
    [SerializeField] int actualPriority; 

    public CinemachineCamera dormCorridorCamera; 
    public CinemachineCamera infirmaryDoorCamera;
    public CinemachineCamera infirmaryCamera;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == ">Spline Corridor1")
        {
            actualPriority = actualCamera.Priority; 
            actualCamera = dormCorridorCamera; 
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spot Infirmary Door")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = infirmaryDoorCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spot Infirmary ")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = infirmaryCamera;
            Destroy(other.gameObject);
        }



            actualCamera.Priority = actualPriority + 1; 


    }
}
