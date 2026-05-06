using Unity.Cinemachine;
using UnityEngine;

public class CameraTriggerer : MonoBehaviour
{
    public CinemachineCamera actualCamera;
    [SerializeField] int actualPriority;

    public CinemachineCamera dormCorridorCamera;
    public CinemachineCamera infirmaryDoorCamera;
    public CinemachineCamera infirmaryCamera;
    public CinemachineCamera infirmaryCorridorCamera;
    public CinemachineCamera infirmaryCorridorChaseCamera;
    public CinemachineCamera firstClassCamera;
    public CinemachineCamera secondClassCamera;
    public CinemachineCamera classCorridorCamera;
    public CinemachineCamera firstToiletCorridorCamera;
    public CinemachineCamera secondToiletCorridorCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == ">Spline Dorm>Infirmary")
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
        else if (other.name == ">Spline Infirmary>Class")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = infirmaryCorridorCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spline Infirmary>Class Chase")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = infirmaryCorridorChaseCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spline Class1")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = firstClassCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spline Class2")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = secondClassCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spline Class>Toilet")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = classCorridorCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spline Toilet>Roof Chase")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = firstToiletCorridorCamera;
            Destroy(other.gameObject);
        }
        else if (other.name == ">Spline Toilet>Roof Chase End")
        {
            actualPriority = actualCamera.Priority;
            actualCamera = secondToiletCorridorCamera;
            Destroy(other.gameObject);
        }


        if (actualCamera != null)
        {
            actualCamera.Priority = actualPriority + 1;
        }
        else
        {
            Debug.LogError($"CameraTriggerer: actualCamera is null after trigger '{other.name}'! Camera reference might be missing in Inspector.");
        }


    }
}
