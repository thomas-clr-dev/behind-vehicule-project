using UnityEngine;

namespace Behind.Progression
{
    public class AnimationTrigger : MonoBehaviour
    {
        public enum ParameterType
        {
            Trigger,
            Bool,
            Int,
            Float
        }

        [Header("References")]
        [SerializeField] private Animator animator;

        [Header("Settings")]
        [SerializeField] private string parameterName;
        [SerializeField] private ParameterType parameterType = ParameterType.Trigger;
        
        [Header("Values")]
        [SerializeField] private bool boolValue;
        [SerializeField] private int intValue;
        [SerializeField] private float floatValue;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EnsureAnimator(other.gameObject);
                
                if (animator != null)
                {
                    ApplyParameter();
                }
            }
        }

        private void EnsureAnimator(GameObject player)
        {
            if (animator != null) return;

            Transform modelChild = player.transform.Find("Character_Model/Player");
            if (modelChild != null)
            {
                animator = modelChild.GetComponent<Animator>();
            }

            if (animator == null)
            {
                animator = player.GetComponentInChildren<Animator>();
            }

            if (animator == null)
            {
                Debug.LogWarning($"[AnimationTrigger] Could not find Animator on player {player.name}", this);
            }
        }

        private void ApplyParameter()
        {
            if (string.IsNullOrEmpty(parameterName)) return;

            switch (parameterType)
            {
                case ParameterType.Trigger:
                    animator.SetTrigger(parameterName);
                    break;
                case ParameterType.Bool:
                    animator.SetBool(parameterName, boolValue);
                    break;
                case ParameterType.Int:
                    animator.SetInteger(parameterName, intValue);
                    break;
                case ParameterType.Float:
                    animator.SetFloat(parameterName, floatValue);
                    break;
            }
        }
    }
}
