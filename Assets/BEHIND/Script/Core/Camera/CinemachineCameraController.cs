using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.TextCore.Text;
[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineCameraController : MonoBehaviour, IEventListener<CameraEvent>
{
    [SerializeField] private Flag cameraFlag;
    //private int basePriority = 10;
    public WheelChairController TargetCharacter { get; set; }

    private CinemachineCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
    }
    public void OnEvent(CameraEvent cameraEvent)
    {
        switch (cameraEvent.EventType)
        {
            case CameraEventTypes.ActivateCamera:
                virtualCamera.Priority = (cameraEvent.CameraFlag == cameraFlag) ? 20 : 10;
                break;
            case CameraEventTypes.SetTargetCharacter:
                if (cameraEvent.TargetCharacter != null)
                {
                    SetTarget(cameraEvent.TargetCharacter);
                }
                break;
            case CameraEventTypes.SetConfiner:

                break;
            case CameraEventTypes.StartFollowing:
                StartFollowing();
                break;
            case CameraEventTypes.StopFollowing:

                break;
            case CameraEventTypes.RefreshPosition:

                break;
            case CameraEventTypes.ResetPriorities:

                break;
            case CameraEventTypes.RefreshAutoFocus:

                break;
            case CameraEventTypes.RefreshRotation:

                break;
        }
    }

    public void StartFollowing()
    {
        virtualCamera.Follow = TargetCharacter.transform;
    }

    public void SetTarget(WheelChairController targetCharacter)
    {
        TargetCharacter = targetCharacter;
    }

    private void OnEnable()
    {
        this.EventStartListening<CameraEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<CameraEvent>();
    }

    public void SetPriority()
    {
      virtualCamera.Priority = 20;
    }
}
