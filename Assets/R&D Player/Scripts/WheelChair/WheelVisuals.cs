using UnityEngine;

[RequireComponent(typeof(WheelChairController))]
public class WheelVisuals : MonoBehaviour
{
    private WheelChairController controller;

    [Header("Visual Transforms")]
    [SerializeField] private Transform rightParentWheel;
    [SerializeField] private Transform leftParentWheel;

    [SerializeField] private Transform rightWheel;
    [SerializeField] private Transform leftWheel;
    [SerializeField] private Transform rightFrontWheel;
    [SerializeField] private Transform leftFrontWheel;

    private void Awake()
    {
        controller = GetComponent<WheelChairController>();
    }

    private void Update()
    {
        if (controller == null) return;

        SyncPosition(controller.RightWheelCollider, rightParentWheel);
        SyncPosition(controller.LeftWheelCollider, leftParentWheel);

        SyncRotation(controller.RightWheelCollider, rightWheel);
        SyncRotation(controller.LeftWheelCollider, leftWheel);

        SyncPosition(controller.RightFrontWheelCollider, rightFrontWheel);
        SyncPosition(controller.LeftFrontWheelCollider, leftFrontWheel);
        SyncRotation(controller.RightFrontWheelCollider, rightFrontWheel);
        SyncRotation(controller.LeftFrontWheelCollider, leftFrontWheel);

    }

    private void SyncPosition(WheelCollider col, Transform vis)
    {
        if (col == null || vis == null) return;
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        vis.position = pos;
    }

    private void SyncRotation(WheelCollider col, Transform vis)
    {
        if (col == null || vis == null) return;
        Quaternion rot;
        Vector3 pos;
        col.GetWorldPose(out pos, out rot);
        vis.rotation = rot;
    }
}
