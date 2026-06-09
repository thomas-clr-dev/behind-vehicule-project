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
        [SerializeField] private Animator[] animators;

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
                if (animators.Length > 0)
                {
                    ApplyParameter();
                }
            }
        }

        private void ApplyParameter()
        {
            if (string.IsNullOrEmpty(parameterName)) return;

            switch (parameterType)
            {
                case ParameterType.Trigger:
                    foreach(Animator animator in animators)
                    {
                        animator.SetTrigger(parameterName);
                    }
                    break;
                case ParameterType.Bool:
                    foreach (Animator animator in animators)
                    {
                        animator.SetBool(parameterName, boolValue);
                    }
                    break;
                case ParameterType.Int:
                    foreach (Animator animator in animators)
                    {
                        animator.SetInteger(parameterName, intValue);
                    }
                    break;
                case ParameterType.Float:
                    foreach (Animator animator in animators)
                    {
                        animator.SetFloat(parameterName, floatValue);
                    }
                    break;
            }
        }
    }
}
