using UnityEngine;

[RequireComponent(typeof(WheelChairController))]
public class WheelVisuals : MonoBehaviour
{
    private WheelChairController controller;

    [Header("Visual Transforms")]
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

        Sync(controller.RightWheelCollider, rightWheel);
        Sync(controller.LeftWheelCollider, leftWheel);
        Sync(controller.RightFrontWheelCollider, rightFrontWheel);
        Sync(controller.LeftFrontWheelCollider, leftFrontWheel);
    }

    private void Sync(WheelCollider col, Transform vis)
    {
        if (col == null || vis == null) return;
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        vis.position = pos;
        vis.rotation = rot;
    }
}