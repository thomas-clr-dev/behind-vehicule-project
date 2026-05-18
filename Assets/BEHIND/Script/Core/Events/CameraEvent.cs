using UnityEngine;
using UnityEngine.TextCore.Text;


public enum CameraEventTypes { ActivateCamera, SetTargetCharacter, SetConfiner, StartFollowing, StopFollowing, RefreshPosition, ResetPriorities, RefreshAutoFocus, RefreshRotation }
public struct CameraEvent 
{

    public CameraEventTypes EventType;
    public WheelChairController TargetCharacter;
    public Collider Bounds;
    public Collider2D Bounds2D;
    public Flag CameraFlag;

    public CameraEvent(CameraEventTypes eventType, WheelChairController targetCharacter = null, Collider bounds = null, Collider2D bounds2D = null, Flag cameraName = null)
    {
        EventType = eventType;
        TargetCharacter = targetCharacter;
        Bounds = bounds;
        Bounds2D = bounds2D;
        CameraFlag = cameraName;
    }

    static CameraEvent e;
    public static void Trigger(CameraEventTypes eventType, WheelChairController targetCharacter = null, Collider bounds = null, Collider2D bounds2D = null, Flag cameraName = null)
    {
        e.EventType = eventType;
        e.Bounds = bounds;
        e.Bounds2D = bounds2D;
        e.TargetCharacter = targetCharacter;
        e.CameraFlag = cameraName;
        EventBus.Publish(e);
    }
}

