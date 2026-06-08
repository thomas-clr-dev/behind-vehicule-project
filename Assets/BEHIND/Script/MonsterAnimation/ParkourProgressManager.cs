using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Behind.Progression
{
    public class ParkourProgressManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerTransform;

        [Header("State")]
        [SerializeField] private int lastHitIndex = -1;
        [SerializeField] private float currentProgress = 0f;

        private List<ParkourCheckpoint> _checkpoints = new List<ParkourCheckpoint>();
        private float[] _segmentDistances;
        private float[] _cumulativeDistances;
        private float _totalDistance;

        public float CurrentProgress => currentProgress;

        private void Awake()
        {
            InitializeCheckpoints();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
                else
                {
                    Debug.LogWarning("[ParkourProgressManager] Player with tag 'Player' not found.");
                }
            }
        }

        private void Update()
        {
            if (playerTransform == null || _checkpoints.Count < 2) return;

            CalculateProgress();
        }

        private void InitializeCheckpoints()
        {
            _checkpoints = Object.FindObjectsByType<ParkourCheckpoint>(FindObjectsSortMode.None).OrderBy(cp => cp.index).ToList();

            if (_checkpoints.Count < 2)
            {
                Debug.LogWarning("[ParkourProgressManager] Not enough checkpoints found to calculate progress.");
                return;
            }

            _segmentDistances = new float[_checkpoints.Count - 1];
            _cumulativeDistances = new float[_checkpoints.Count];
            _totalDistance = 0f;

            _cumulativeDistances[0] = 0f;
            for (int i = 0; i < _checkpoints.Count - 1; i++)
            {
                float dist = Vector3.Distance(_checkpoints[i].transform.position, _checkpoints[i + 1].transform.position);
                _segmentDistances[i] = dist;
                _totalDistance += dist;
                _cumulativeDistances[i + 1] = _totalDistance;
            }
        }

        private void CalculateProgress()
        {
            if (lastHitIndex < 0)
            {
                currentProgress = 0f;
                return;
            }

            if (lastHitIndex >= _checkpoints.Count - 1)
            {
                currentProgress = 1f;
                return;
            }

            Vector3 startPos = _checkpoints[lastHitIndex].transform.position;
            Vector3 endPos = _checkpoints[lastHitIndex + 1].transform.position;
            Vector3 playerPos = playerTransform.position;

            Vector3 segmentDir = (endPos - startPos);
            float segmentLength = _segmentDistances[lastHitIndex];
            
            if (segmentLength <= 0.001f)
            {
                currentProgress = _cumulativeDistances[lastHitIndex + 1] / _totalDistance;
                return;
            }

            Vector3 playerToStart = playerPos - startPos;
            float projection = Vector3.Dot(playerToStart, segmentDir.normalized);
            
            float clampedProjection = Mathf.Clamp(projection, 0f, segmentLength);
            
            float distanceCovered = _cumulativeDistances[lastHitIndex] + clampedProjection;
            currentProgress = Mathf.Clamp01(distanceCovered / _totalDistance);
        }

        public void OnCheckpointHit(ParkourCheckpoint checkpoint)
        {
            if (checkpoint.index > lastHitIndex)
            {
                lastHitIndex = checkpoint.index;
                

                if (lastHitIndex >= _checkpoints.Count - 1)
                {
                    currentProgress = 1f;
                }
            }
        }
    }
}
