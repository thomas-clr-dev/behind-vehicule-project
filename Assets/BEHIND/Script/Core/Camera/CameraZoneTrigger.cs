using UnityEngine;
using UnityEngine.Events;

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private Flag cameraToActivate;
    [SerializeField] private bool destroyAfterUse = true;

    public UnityEvent Event;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraEvent.Trigger(CameraEventTypes.ActivateCamera, null, null, null, cameraToActivate);

            Event?.Invoke();

            if (destroyAfterUse) Destroy(gameObject);
        }
    }
}