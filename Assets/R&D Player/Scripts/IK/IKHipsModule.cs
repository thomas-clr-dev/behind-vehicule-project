using UnityEngine;

public class IKHipsModule : MonoBehaviour
{

    [SerializeField] private Transform hipsTransform;
    [SerializeField] private Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hipsTransform.position = target.position;
    }

   
}
