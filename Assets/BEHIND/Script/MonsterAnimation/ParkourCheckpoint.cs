using UnityEngine;

namespace Behind.Progression
{
    public class ParkourCheckpoint : MonoBehaviour
    {
        [Header("Settings")]
        public int index;

        private ParkourProgressManager _manager;

        private void Start()
        {
            _manager = Object.FindAnyObjectByType<ParkourProgressManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_manager != null)
                {
                    _manager.OnCheckpointHit(this);
                }
            }
        }
    }
}
