using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private Flag cameraToActivate;
    [SerializeField] private bool destroyAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraEvent.Trigger(CameraEventTypes.ActivateCamera, null, null, null, cameraToActivate);

            if (destroyAfterUse) Destroy(gameObject);
        }
    }
}